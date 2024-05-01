using Models.Dtos;
using Models.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Servises.Interfaces;
using Models.Abstractions;
using Microsoft.Extensions.Configuration;
using Mapster;
using Models.Exceptions;
using System.Security.Claims;

namespace Servises.Services;

public class AuthorizationService(IUnitOfWork _unitOfWork, ITokenService tokenService, IConfiguration configuration) : 
    BaseService(_unitOfWork), IAuthorizationService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<string> RegisterUserAsync(RegisterUserDto registerDto)
    {

        UserAuthorizationInfo? ai = await unitOfWork.UserAuthorizationRepository.GetByEmailAsync(registerDto.Email);
        if (ai is not null)
            throw new BadRequestException("User with this email already exist");
        CreatePasswordHashAndSalt(registerDto.Password, out byte[] hash, out byte[] salt);
        var token = _tokenService.GenerateToken();
        ai = new()
        {
            Email = registerDto.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            EmailConfirmToken = token
        };
        User user = registerDto.Adapt<User>();
        user.AuthorizationInfo = ai;
        if (registerDto.ImageDto is not null)
        {
            using MemoryStream memoryStream = new();
            await registerDto.ImageDto.File.CopyToAsync(memoryStream);
            user.UserImage = new UserImage()
            {
                UserId = user.Id,
                FileName = registerDto.ImageDto.File.FileName,
                FileType = registerDto.ImageDto.File.ContentType,
                Bytes = memoryStream.ToArray()
            };
        }
        await unitOfWork.UserRepository.AddAsync(user);
        await unitOfWork.SaveAllAsync();

        return token;
    }
    public async Task<string> RegisterAdminAsync(RegisterAdminDto registerDto)
    {
        AdminAuthorizationInfo? ai = await unitOfWork.AdminAuthorizationRepository.GetByEmailAsync(registerDto.Email);
        if (ai is not null)
            throw new BadRequestException("Admin with this email already exist");
        CreatePasswordHashAndSalt(registerDto.Password, out byte[] hash, out byte[] salt);
        var token = _tokenService.GenerateToken();
        ai = new()
        {
            Email = registerDto.Email,
            PasswordHash = hash,
            PasswordSalt = salt,
            EmailConfirmToken = token
        };
        Administrator admin = registerDto.Adapt<Administrator>();
        admin.AuthorizationInfo = ai;


        await unitOfWork.AdministratorRepository.AddAsync(admin);
        await unitOfWork.SaveAllAsync();

        return token;
    }
    public async Task<LoginResponseDto> RefreshUser(RefreshDto refreshModel)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(refreshModel.AccessToken);
        if (principal?.Identity?.Name is null)
            throw new BadRequestException("Invalid jwt.");

        UserAuthorizationInfo userAuthorizationInfo = await unitOfWork.UserAuthorizationRepository.GetByEmailAsync(principal.Identity.Name)
           ?? throw new NotFoundException(nameof(User));

        if (userAuthorizationInfo.RefreshToken != refreshModel.RefreshToken || userAuthorizationInfo.RefreshTokenExpiry < DateTime.UtcNow)
            throw new BadRequestException("Invalid refresh token.");

        var id = Guid.Parse(principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        var user = await unitOfWork.UserRepository.GetByIdAsync(id)
          ??  throw new NotFoundException(nameof(User));
        if (user.State==StateType.IsBanned)
            throw new BadRequestException($"{nameof(User)} is banned");
        if (user.State == StateType.IsBlocked)
            throw new BadRequestException($"{nameof(User)} is blocked");

        var token = await _tokenService.GenerateJwt(id, principal.Identity.Name, principal.Identity.AuthenticationType
            ?? throw new BadRequestException("Invalid jwt."));

        return new(
            JwtToken: new JwtSecurityTokenHandler().WriteToken(token),
            Expiration: token.ValidTo,
            RefreshToken: refreshModel.RefreshToken,
            Id: id
        );
    }
    public async Task<LoginResponseDto> RefreshAdmin(RefreshDto refreshModel)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(refreshModel.AccessToken);
        if (principal?.Identity?.Name is null)
            throw new BadRequestException("Invalid jwt.");

        AdminAuthorizationInfo adminAuthorizationInfo = await unitOfWork.AdminAuthorizationRepository.GetByEmailAsync(principal.Identity.Name)
        ?? throw new NotFoundException(nameof(Administrator));

        if (adminAuthorizationInfo.RefreshToken != refreshModel.RefreshToken || adminAuthorizationInfo.RefreshTokenExpiry < DateTime.UtcNow)
            throw new BadRequestException("Invalid refresh token.");

        var id = Guid.Parse(principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

        var Admin = await unitOfWork.AdministratorRepository.GetByIdAsync(id);
        if (Admin is null)
            throw new NotFoundException(nameof(Admin));

        var token = await _tokenService.GenerateJwt(id, principal.Identity.Name, principal.Identity.AuthenticationType
            ?? throw new BadRequestException("Invalid jwt."));

        return new(
            JwtToken: new JwtSecurityTokenHandler().WriteToken(token),
            Expiration: token.ValidTo,
            RefreshToken: refreshModel.RefreshToken,
            Id: id
        );
    }
    public static void CreatePasswordHashAndSalt(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512();
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
    public async Task<LoginResponseDto> LoginUserAsync(LoginDto loginModel)
    {
        AuthorizationInfo authorizationInfo = await unitOfWork.UserAuthorizationRepository.GetByEmailAsync(loginModel.Email)
          ??  throw new NotFoundException(nameof(User));
        if (!CheckPassword(loginModel.Password, authorizationInfo.PasswordHash, authorizationInfo.PasswordSalt))
            throw new BadRequestException("Invalid password.");
        if (!authorizationInfo.IsEmailConfirmed)
            throw new BadRequestException("Email is not confirmed.");
        JwtSecurityToken? jwt;
        User user = await unitOfWork.UserRepository.GetByIdAsync(authorizationInfo.UserId)
           ?? throw new NotFoundException(nameof(User));
        jwt = await _tokenService.GenerateJwt(authorizationInfo.UserId, loginModel.Email, user.State.ToString() + "User");
        var refreshToken = _tokenService.GenerateToken();

        authorizationInfo.RefreshToken = refreshToken;
        authorizationInfo.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await unitOfWork.SaveAllAsync();
        return new(
            JwtToken: new JwtSecurityTokenHandler().WriteToken(jwt),
            Expiration: jwt.ValidTo,
            RefreshToken: refreshToken,
            Id: authorizationInfo.UserId
        );
    }
    public async Task<LoginResponseDto> LoginAdminAsync(LoginDto loginModel)
    {
        AuthorizationInfo authorizationInfo = await unitOfWork.AdminAuthorizationRepository.GetByEmailAsync(loginModel.Email)
           ?? throw new NotFoundException(nameof(User));
        if (!CheckPassword(loginModel.Password, authorizationInfo.PasswordHash, authorizationInfo.PasswordSalt))
            throw new BadRequestException("Invalid password.");
        if (!authorizationInfo.IsEmailConfirmed)
            throw new BadRequestException("Email is not confirmed.");
        JwtSecurityToken? jwt;
        jwt = await _tokenService.GenerateJwt(authorizationInfo.UserId, loginModel.Email, "Admin");
        var refreshToken = _tokenService.GenerateToken();

        authorizationInfo.RefreshToken = refreshToken;
        authorizationInfo.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await unitOfWork.SaveAllAsync();
        return new(
            JwtToken: new JwtSecurityTokenHandler().WriteToken(jwt),
            Expiration: jwt.ValidTo,
            RefreshToken: refreshToken,
            Id: authorizationInfo.UserId
        );
    }

    public static bool CheckPassword(string password, byte[] userPasswordHash, byte[] userPasswordSalt)
    {
        using var hmac = new HMACSHA512(userPasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(userPasswordHash);
    }

    public async Task<string> UserForgotPassword(string email)
    {
        UserAuthorizationInfo authorizationInfo = await unitOfWork.UserAuthorizationRepository.GetByEmailAsync(email)
         ??   throw new NotFoundException(nameof(User));

        var token = _tokenService.GenerateToken();

        authorizationInfo.ResetPasswordToken = token;
        authorizationInfo.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);
        await unitOfWork.SaveAllAsync();
        return token;
    }

    public async Task UserResetPassword(ResetPasswordDto model)
    {
        UserAuthorizationInfo? authorizationInfo = await unitOfWork.UserAuthorizationRepository.GetByEmailAsync(model.Email)
            ?? throw new NotFoundException(nameof(User));

        if (authorizationInfo.ResetPasswordToken != model.ResetToken)
            throw new BadRequestException("Invalid token.");

        if (authorizationInfo.ResetPasswordTokenExpiry < DateTime.UtcNow)
            throw new BadRequestException("Reset time is out.");

        CreatePasswordHashAndSalt(model.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);

        authorizationInfo.PasswordHash = passwordHash;
        authorizationInfo.PasswordSalt = passwordSalt;
        authorizationInfo.ResetPasswordToken = null;
        authorizationInfo.ResetPasswordTokenExpiry = null;

        await unitOfWork.SaveAllAsync();
    }
}

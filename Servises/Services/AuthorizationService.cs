using Microsoft.IdentityModel.Tokens;
using Models.Dtos;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Servises.Interfaces;
using Models.Abstractions;
using Microsoft.Extensions.Configuration;
using Mapster;
using Models.Exceptions;

namespace Servises.Services;

public class AuthorizationService : BaseService, IAuthorizationService
{
    private IConfiguration _configuration;
    private ITokenService _tokenService;
    public AuthorizationService(IUnitOfWork _unitOfWork,ITokenService tokenService, IConfiguration configuration) : base(_unitOfWork) 
    {
        _configuration = configuration;
        _tokenService = tokenService;
    }
    public async Task<string> RegisterUserAsync(RegisterUserDto registerDto)
    {
        
        UserAuthorizationInfo? ai = await unitOfWork.UserAuthorizationRepository.GetByEmailAsync(registerDto.Email);
        if (ai is not null)
            throw new BadRequestException("User with this email already exist");
        ai = new();
        ai.Email = registerDto.Email;
        CreatePasswordHashAndSalt(registerDto.Password, out byte[] hash, out byte[] salt);
        var token = _tokenService.GenerateToken();
        ai.PasswordHash = hash;
        ai.PasswordSalt = salt;
        ai.EmailConfirmToken = token;
        User user = registerDto.Adapt<User>();
        user.AuthorizationInfo = ai;

        await unitOfWork.UserRepository.AddAsync(user);
        await unitOfWork.SaveAllAsync();

        return token;
    }
    public async Task<string> RegisterAdminAsync(RegisterAdminDto registerDto)
    {
        AdminAuthorizationInfo? ai = await unitOfWork.AdminAuthorizationRepository.GetByEmailAsync(registerDto.Email);
        if (ai is not null)
            throw new BadRequestException("Admin with this email already exist");
        ai = new();
        ai.Email = registerDto.Email;
        CreatePasswordHashAndSalt(registerDto.Password, out byte[] hash, out byte[] salt);
        var token = _tokenService.GenerateToken();
        ai.PasswordHash = hash;
        ai.PasswordSalt = salt;
        ai.EmailConfirmToken = token;
        Administrator admin = registerDto.Adapt<Administrator>();
        admin.Id= Guid.NewGuid();
        ai.UserId=admin.Id;
        admin.AuthorizationInfo = ai;


        await unitOfWork.AdministratorRepository.AddAsync(admin);
        await unitOfWork.SaveAllAsync();

        return token;
    }
    public static void CreatePasswordHashAndSalt(string password, out byte[] userPasswordHash, out byte[] userPasswordSalt)
    {
        using var hmac = new HMACSHA512();
        userPasswordSalt = hmac.Key;
        userPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }
    public async Task<LoginResponseDto> LoginUserAsync(LoginDto loginModel)
    {
        UserAuthorizationInfo? authorizationInfo = await unitOfWork.UserAuthorizationRepository.GetByEmailAsync(loginModel.Email);
        if (authorizationInfo is null)
            throw new NotFoundException(nameof(User));
        if (!CheckPassword(loginModel.Password, authorizationInfo.PasswordHash, authorizationInfo.PasswordSalt))
            throw new BadRequestException("Invalid password.");
        JwtSecurityToken? jwt;
        User? user = await unitOfWork.UserRepository.GetByIdAsync(authorizationInfo.UserId);
        if (user is null) 
            throw new NotFoundException(nameof(User));
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
        AuthorizationInfo? authorizationInfo = await unitOfWork.AdminAuthorizationRepository.GetByEmailAsync(loginModel.Email);
        if (authorizationInfo is null)
            throw new NotFoundException(nameof(User));
        if (!CheckPassword(loginModel.Password, authorizationInfo.PasswordHash, authorizationInfo.PasswordSalt))
            throw new BadRequestException("Invalid password.");
        JwtSecurityToken? jwt;
        jwt = await _tokenService.GenerateJwt(authorizationInfo.UserId,loginModel.Email, "Admin");
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
}

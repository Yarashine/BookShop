using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Abstractions;
using Models.Dtos;
using Models.Entities;
using Servises.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Models.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Servises.Services;

public class TokenService(IUnitOfWork _unitOfWork, IConfiguration configuration) : BaseService(_unitOfWork), ITokenService
{
    private readonly IConfiguration _configuration = configuration;

    public string GenerateToken() => Guid.NewGuid().ToString();

    public Task<JwtSecurityToken> GenerateJwt(Guid userId,string email, string NameOfRole)
    {

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString() ),
            new(ClaimTypes.Role, NameOfRole),
            new(ClaimTypes.Name, email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]
            ?? throw new InvalidOperationException("Key not configured")));

        var now = DateTime.UtcNow;

        var jwt = new JwtSecurityToken(
            issuer: _configuration["Jwt:ValidIssuer"],
            audience: _configuration["Jwt:ValidAudience"],
            notBefore: now,
            claims: claims,
            expires: now.AddHours(Convert.ToDouble(_configuration["Jwt:DurationInHours"])),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return Task.FromResult(jwt);
    }
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var key = _configuration["JWT:Key"] ?? throw new InvalidOperationException("Key not configured");

        var validation = new TokenValidationParameters
        {
            ValidIssuer = _configuration["JWT:ValidIssuer"],
            ValidAudience = _configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        };

        return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
    }

    public async Task RevokeUserRefreshTokenByEmail(string userEmail)
    {
        if (userEmail.IsNullOrEmpty())
            throw new UnauthorizedException("Invalid email.");

        UserAuthorizationInfo? authorizationInfo = await unitOfWork.UserAuthorizationRepository.GetByEmailAsync(userEmail)
            ?? throw new NotFoundException(nameof(User));

        authorizationInfo.RefreshToken = null;
        authorizationInfo.RefreshTokenExpiry = null;

        await unitOfWork.SaveAllAsync();
    }
    public async Task RevokeAdminRefreshTokenByEmail(string adminEmail)
    {
        if (adminEmail.IsNullOrEmpty())
            throw new UnauthorizedException("Invalid email.");

        AdminAuthorizationInfo? authorizationInfo = await unitOfWork.AdminAuthorizationRepository.GetByEmailAsync(adminEmail)
            ?? throw new NotFoundException(nameof(User));

        authorizationInfo.RefreshToken = null;
        authorizationInfo.RefreshTokenExpiry = null;

        await unitOfWork.SaveAllAsync();
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.Abstractions;
using Models.Dtos;
using Models.Entities;
using Repositories.Repositories;
using Servises.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Servises.Services;

public class TokenService : BaseService, ITokenService
{
    private IConfiguration _configuration;
    public TokenService(IUnitOfWork _unitOfWork, IConfiguration configuration) : base(_unitOfWork)
    { _configuration = configuration; }

    public string GenerateToken() => Guid.NewGuid().ToString();

    public async Task<JwtSecurityToken?> GenerateJwt(Guid userId,string email, string NameOfRole)
    {
        //var user = await _repository.GetByEmailAsync(email) ?? throw new NotFoundException(nameof(User));
        /*AuthorizationInfo? authorizationInfo = await unitOfWork.AuthorizationRepository.GetByEmailAsync(email);
        if (authorizationInfo is null) return null;*/


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

        return jwt;
    }

    /*public async Task<LoginResponseDto> Refresh(RefreshDto refreshModel)
    {
        var principal = GetPrincipalFromExpiredToken(refreshModel.AccessToken);
        if (principal?.Identity?.Name is null)
            throw new BadRequestException("Invalid jwt.");

        var user = await _repository.GetByEmailAsync(principal.Identity.Name)
            ?? throw new NotFoundException(nameof(User));

        if (user.RefreshToken != refreshModel.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new BadRequestException("Invalid refresh token.");

        var token = await GenerateJwt(principal.Identity.Name);

        return new(
            JwtToken: new JwtSecurityTokenHandler().WriteToken(token),
            Expiration: token.ValidTo,
            RefreshToken: refreshModel.RefreshToken,
            UserId: user.Id
        );
    }
    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
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

    public async Task RevokeRefreshTokenByEmail(string userEmail)
    {
        if (userEmail.IsNullOrEmpty())
            throw new UnauthorizedException("Invalid email.");

        var user = await _repository.GetByEmailAsync(userEmail)
            ?? throw new NotFoundException(nameof(User));

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;

        await _repository.UpdateAsync(user);
    }*/
}

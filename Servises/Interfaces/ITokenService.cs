using Models.Dtos;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Servises.Interfaces;

public interface ITokenService
{
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    Task RevokeUserRefreshTokenByEmail(string userEmail);
    Task RevokeAdminRefreshTokenByEmail(string adminEmail);
    string GenerateToken();
    Task<JwtSecurityToken> GenerateJwt(Guid userId, string email, string NameOfRole);
}

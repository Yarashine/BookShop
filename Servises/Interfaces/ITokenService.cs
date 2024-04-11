using Models.Dtos;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servises.Interfaces;

public interface ITokenService
{/*
    Task<LoginResponseDto> Refresh(RefreshDto refreshModel);
    Task RevokeRefreshTokenByEmail(string userEmail);*/
    string GenerateToken();
    Task<JwtSecurityToken> GenerateJwt(Guid userId, string email, string NameOfRole);
}

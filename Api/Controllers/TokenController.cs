/*using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Servises.Interfaces;

namespace Api.Controllers;

[Route("api/tokens")]
[ApiController]
public class TokenController : Controller
{
    private readonly ITokenService _tokenService;
    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }
*//*
    [Authorize]
    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke()
    {
        var userEmail = HttpContext.User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail)) 
        {
            return BadRequest();
        }
        await _tokenService.RevokeUserRefreshTokenByEmail(userEmail);
        return Ok();
    }*//*

}
*/
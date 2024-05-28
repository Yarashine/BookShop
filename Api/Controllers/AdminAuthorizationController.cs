using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Servises.Interfaces;
using Models.Entities;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.UI.Services;
using Servises.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json.Linq;


namespace Api.Controllers;
//[Authorize(Policy = "IsNotBlocked")]
[AllowAnonymous]
[ApiController]
[Route("/api/authorization")]
public class AdminAuthorizationController(Servises.Interfaces.IAuthorizationService _authorizationService,
    IEmailService _emailService, ITokenService _tokenService) : Controller
{

    [HttpPost("register/admin")]
    public async Task<ActionResult<string?>> RegisterAdmin([FromBody] RegisterAdminDto registerDto)
    {
        string? result = await _authorizationService.RegisterAdminAsync(registerDto);
        if (string.IsNullOrEmpty(result))
            return BadRequest();
       /* var emailBodyUrl = Request.Scheme + "://" + Request.Host +
            Url.Action("confirmadminemail", "adminauthorization", new { email = registerDto.Email, token = result });
        await emailService.SendConfirmEmail(registerDto.Email, emailBodyUrl);*/
        return result;
    }

    [HttpPost("login/admin")]
    public async Task<ActionResult<LoginResponseDto?>> LoginAdmin([FromBody] LoginDto loginDto)
    {
        LoginResponseDto? result = await _authorizationService.LoginAdminAsync(loginDto);
        return result;
    }

    /*[HttpPost("refresh/admin")]
    public async Task<ActionResult<LoginResponseDto>> RefreshAdmin([FromBody] RefreshDto model)
    {
        var response = await authorizationService.RefreshAdmin(model);
        return Ok(response);
    }

    [HttpGet("confirmemail/admin")]
    public async Task<IActionResult> ConfirmAdminEmail([EmailAddress] string email, string token)
    {
        await emailService.ConfirmAdminEmailByToken(email, token);
        return Ok("Confirmation was successful.");

    }
    [HttpPost("logout/admin")]
    public async Task<IActionResult> LogoutAdmin()
    {
        var userEmail = HttpContext.User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            return BadRequest();
        }
        await tokenService.RevokeAdminRefreshTokenByEmail(userEmail);
        //await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
        return Ok();
    }*/
}

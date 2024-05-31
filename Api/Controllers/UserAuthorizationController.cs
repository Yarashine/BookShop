using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Servises.Interfaces;
using Models.Entities;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;


namespace Api.Controllers;
[AllowAnonymous]
[ApiController]
[Route("/api/authorization")]
public class AuthorizationController(Servises.Interfaces.IAuthorizationService _authorizationService,
    IEmailService _emailService, ITokenService _tokenService) : Controller
{
    [AllowAnonymous]
    [HttpPost("register/admin")]
    public async Task<ActionResult<string?>> RegisterAdmin([FromBody] RegisterAdminDto registerDto)
    {
        string scheme = HttpContext.Request.Scheme;
        string host = HttpContext.Request.Host.ToString();
        string url = $"{scheme}://{host}/api/authorization/register/admin";
        Console.WriteLine($"Register URL: {url}");
        string? result = await _authorizationService.RegisterAdminAsync(registerDto);
        if (string.IsNullOrEmpty(result))
            return BadRequest();
        return result;
    }

    [HttpPost("register/user")]
    public async Task<ActionResult<string?>> RegisterUser([FromForm] RegisterUserDto registerDto)
    {
        string? result = await _authorizationService.RegisterUserAsync(registerDto);
        if (string.IsNullOrEmpty(result))
            return BadRequest();
        var emailBodyUrl = Request.Scheme + "://" + Request.Host +
            Url.Action("confirmemail", "authorization", new { email = registerDto.Email, token = result });
        await _emailService.SendConfirmEmail(registerDto.Email, emailBodyUrl);
        return result;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto?>> LoginUser([FromBody] LoginDto loginDto)
    {
        LoginResponseDto? result = await _authorizationService.LoginAsync(loginDto);
        if (result is null)
            return BadRequest();
        return result;
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDto>> Refresh([FromBody] RefreshDto model)
    {
        var response = await _authorizationService.Refresh(model);
        return Ok(response);
    }

    [HttpGet("confirmemail")]
    public async Task<IActionResult> ConfirmEmail([EmailAddress] string email, string token)
    {
        await _emailService.ConfirmUserEmailByToken(email, token);
        return Ok("Confirmation was successful.");
    }

    [HttpPost("forgotpassword")]
    public async Task<IActionResult> ForgotPassword([EmailAddress] string email)
    {

        var token = await _authorizationService.UserForgotPassword(email);
        var emailBodyUrl = Request.Scheme + "://" + Request.Host + 
            Url.Action("resetpassword", "authorization", new { email, token });
        await _emailService.SendResetPasswordEmail(email, emailBodyUrl);
        return Ok($"Check {email} email. You may now reset your password whithin 1 hour.");
    }

    [HttpGet("resetpassword")]
    public ActionResult<ResetPasswordDto> ResetPassword([EmailAddress] string email, string token)
    {
        var model = new ResetPasswordDto(email, "", token);
        return Ok(model);
    }

    [HttpPost("resetpassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        await _authorizationService.UserResetPassword(model);
        return Ok("Password has been successfully changed.");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userEmail = HttpContext.User.Identity?.Name;
        if (string.IsNullOrEmpty(userEmail))
        {
            return BadRequest();
        }
        await _tokenService.RevokeUserRefreshTokenByEmail(userEmail);
        //await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
        return Ok();
    }
}

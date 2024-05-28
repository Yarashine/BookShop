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
public class UserAuthorizationController(Servises.Interfaces.IAuthorizationService _authorizationService,
    IEmailService _emailService, ITokenService _tokenService) : Controller
{

    [HttpPost("register/user")]
    public async Task<ActionResult<string?>> RegisterUser([FromForm] RegisterUserDto registerDto)
    {
        string? result = await _authorizationService.RegisterUserAsync(registerDto);
        if (string.IsNullOrEmpty(result))
            return BadRequest();
        var emailBodyUrl = Request.Scheme + "://" + Request.Host +
            Url.Action("confirmuseremail", "userauthorization", new { email = registerDto.Email, token = result });
        await _emailService.SendConfirmEmail(registerDto.Email, emailBodyUrl);
        return result;
    }

    [HttpPost("login/user")]
    public async Task<ActionResult<LoginResponseDto?>> LoginUser([FromBody] LoginDto loginDto)
    {
        LoginResponseDto? result = await _authorizationService.LoginUserAsync(loginDto);
        if (result is null)
            return BadRequest();
        return result;
    }

    [HttpPost("refresh/user")]
    public async Task<ActionResult<LoginResponseDto>> RefreshUser([FromBody] RefreshDto model)
    {
        var response = await _authorizationService.RefreshUser(model);
        return Ok(response);
    }

    [HttpGet("confirmemail/user")]
    public async Task<IActionResult> ConfirmUserEmail([EmailAddress] string email, string token)
    {
        await _emailService.ConfirmUserEmailByToken(email, token);
        return Ok("Confirmation was successful.");
    }

    [HttpPost("forgotpassword/user")]
    public async Task<IActionResult> ForgotPassword([EmailAddress] string email)
    {

        var token = await _authorizationService.UserForgotPassword(email);
        var emailBodyUrl = Request.Scheme + "://" + Request.Host + 
            Url.Action("resetpassword", "userauthorization", new { email, token });
        await _emailService.SendResetPasswordEmail(email, emailBodyUrl);
        return Ok($"Check {email} email. You may now reset your password whithin 1 hour.");
    }

    [HttpGet("resetpassword/user")]
    public ActionResult<ResetPasswordDto> ResetPassword([EmailAddress] string email, string token)
    {
        var model = new ResetPasswordDto(email, "", token);
        return Ok(model);
    }

    [HttpPost("resetpassword/user")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        await _authorizationService.UserResetPassword(model);
        return Ok("Password has been successfully changed.");
    }

    [HttpPost("logout/user")]
    public async Task<IActionResult> LogoutUser()
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

using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Servises.Interfaces;
using Models.Entities;
using Microsoft.AspNetCore.Authorization;


namespace Api.Controllers;
//[Authorize(Policy = "IsNotBlocked")]
[AllowAnonymous]
[ApiController]
[Route("/api")]
public class AuthorizationController : Controller
{
    private readonly Servises.Interfaces.IAuthorizationService authorizationService;

    public AuthorizationController(Servises.Interfaces.IAuthorizationService _authorizationService)
    {
        authorizationService = _authorizationService;
    }
    /*[HttpPost("register/user")]
    public async Task<ActionResult<string?>> RegisterUser([FromForm] RegisterUserDto registerDto)
    {
        string? result = await authorizationService.RegisterUserAsync(registerDto);

        return result;
    }*/
    [HttpPost("register/user")]
    public async Task<ActionResult<LoginResponseDto?>> RegisterUser([FromForm] RegisterUserDto registerDto)
    {
        string? result = await authorizationService.RegisterUserAsync(registerDto);
        LoginResponseDto? result2 = await authorizationService.LoginUserAsync(new(registerDto.Email, registerDto.Password));
        return result2;
    }
    [HttpPost("login/user")]
    public async Task<ActionResult<LoginResponseDto?>> LoginUser([FromBody] LoginDto loginDto)
    {
        LoginResponseDto? result = await authorizationService.LoginUserAsync(loginDto);
        return result;
    }
    [HttpPost("login/admin")]
    public async Task<ActionResult<LoginResponseDto?>> LoginAdmin([FromBody] LoginDto loginDto)
    {
        LoginResponseDto? result = await authorizationService.LoginAdminAsync(loginDto);
        return result;
    }
    [HttpPost("register/admin")]
    /*public async Task<ActionResult<string?>> RegisterAdmin([FromBody] RegisterAdminDto registerDto)
    {
        string? result = await authorizationService.RegisterAdminAsync(registerDto);

        return result;
    }*/
    public async Task<ActionResult<LoginResponseDto?>> RegisterAdmin([FromBody] RegisterAdminDto registerDto)
    {
        string? result = await authorizationService.RegisterAdminAsync(registerDto);
        LoginResponseDto? result2 = await authorizationService.LoginAdminAsync(new(registerDto.Email, registerDto.Password));
        return result2;
    }


}

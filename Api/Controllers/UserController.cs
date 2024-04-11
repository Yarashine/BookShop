using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Servises.Interfaces;
using Models.Entities;
using System.Runtime.InteropServices;
using Mapster;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;


[Authorize(Policy = "IsNotBlocked")]
[Authorize(Policy = "IsNotBunned")]
[ApiController]
[Route("/api")]
public class UserController : Controller
{
    private readonly IUserService userService;
    public UserController(IUserService _userService)
    {
        userService = _userService;
    }
    /*[HttpPost("user")]
    public async Task<IActionResult> AddUser([FromForm] UserDto userDto)
    {
        bool result = await userService.AddUserAsync(userDto);

        if (!result)
            return BadRequest();

        return Ok();
    }*/
    [HttpGet("users")]
    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        return await userService.GetAllUsersAsync();
    }
   

}

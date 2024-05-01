using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Servises.Interfaces;
using Models.Entities;
using System.Runtime.InteropServices;
using Mapster;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using Servises.Services;

namespace Api.Controllers;


//[Authorize(Policy = "IsNotBlocked")]
//[Authorize(Policy = "IsNotBunned")]
[ApiController]
[Route("/api")]
public class UserController(IUserService _userService) : Controller
{
    private readonly IUserService userService = _userService;

    [HttpGet("users")]
    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        return await userService.GetAllUsersAsync();
    }
    [HttpPost("card/add")]
    public async Task<IActionResult> AddCard([FromForm] CardDto card, Guid userId)
    {
        bool result = await userService.AddBankAccount(userId, card);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpPut("card/delete")]
    public async Task<IActionResult> DeleteCard(Guid userId)
    {
        bool result = await userService.DeleteBankAccount(userId);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpPut("card/update")]
    public async Task<IActionResult> UpdateCard([FromForm] CardDto card, Guid userId)
    {
        bool result = await userService.UpdateBankAccount(userId, card);

        if (!result)
            return BadRequest();

        return Ok();
    }


}

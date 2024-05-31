using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Servises.Interfaces;
using Models.Entities;
using System.Runtime.InteropServices;
using Mapster;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using Servises.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Api.Controllers;

[ApiController]
[Route("/api")]
public class UserController(IUserService _userService) : Controller
{
    [Authorize(Roles = "IsExistedUser,IsBlockedUser")]
    [HttpPost("user/unban")]
    public async Task<IActionResult> UnbanRequest([FromForm] string? description)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _userService.UnbanRequestAsync(userId, description);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser,IsBlockedUser")]
    [HttpPut("user/update")]
    public async Task<IActionResult> UpdateUserAsync([FromForm] UpdateUserDto user)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _userService.UpdateUserAsync(userId, user);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser,IsBlockedUser")]
    [HttpDelete("user/delete")]
    public async Task<IActionResult> DeleteUserAsync()
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _userService.DeleteUserAsync(userId);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser,IsBlockedUser")]
    [HttpGet("user")]
    public async Task<IActionResult> GetByIdAsync()
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        var user = await _userService.GetByIdAsync(userId);
        return Ok(user);
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpGet("user/favorites")]
    public async Task<IActionResult> GetFavoritesAsync()
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        var favorites = await _userService.GetFavoritesAsync(userId);
        return Ok(favorites);
    }

    [Authorize(Roles = "User")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpGet("user/library")]
    public async Task<IActionResult> GetLibraryAsync()
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        var library = await _userService.GetLibraryAsync(userId);
        return Ok(library);
    }

    [Authorize(Roles = "IsExistedUser,IsBlockedUser")]
    [HttpGet("user/bookstosell")]
    public async Task<IActionResult> GetBookToSellAsync()
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        var bookToSell = await _userService.GetBookToSellAsync(userId);
        return Ok(bookToSell);
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpGet("user/purchasedbook")]
    public async Task<IActionResult> GetPurchasedBookAsync()
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        var purchasedBook = await _userService.GetPurchasedBookAsync(userId);
        return Ok(purchasedBook);
    }
}

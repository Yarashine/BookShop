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
    [Authorize(Roles = "IsExistedUser")]
    [HttpPost("user/unban")]
    public async Task<IActionResult> UnbanRequest([FromForm] Guid userId, string? description)
    {
        await _userService.UnbanRequestAsync(userId, description);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [HttpPut("user/update")]
    public async Task<IActionResult> UpdateUserAsync([FromForm] Guid userId, [FromForm] UpdateUserDto user)
    {
        await _userService.UpdateUserAsync(userId, user);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [HttpDelete("user/delete/{id}")]
    public async Task<IActionResult> DeleteUserAsync([FromRoute] Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return Ok();
    }
    //[Authorize(Roles = "User,Admin")]
    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return Ok(user);
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpGet("user/favorites/{id}")]
    public async Task<IActionResult> GetFavoritesAsync([FromRoute] Guid id)
    {
        var favorites = await _userService.GetFavoritesAsync(id);
        return Ok(favorites);
    }

    [Authorize(Roles = "User")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpGet("user/library/{id}")]
    public async Task<IActionResult> GetLibraryAsync([FromRoute] Guid id)
    {
        var library = await _userService.GetLibraryAsync(id);
        return Ok(library);
    }

    [Authorize(Roles = "IsExistedUser")]
    [HttpGet("user/bookstosell/{id}")]
    public async Task<IActionResult> GetBookToSellAsync([FromRoute] Guid id)
    {
        var bookToSell = await _userService.GetBookToSellAsync(id);
        return Ok(bookToSell);
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpGet("user/purchasedbook/{id}")]
    public async Task<IActionResult> GetPurchasedBookAsync([FromRoute] Guid id)
    {
        var purchasedBook = await _userService.GetPurchasedBookAsync(id);
        return Ok(purchasedBook);
    }
}

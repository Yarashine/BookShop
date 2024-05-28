using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Servises.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Servises.Services;

namespace Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("/api")]
public class AdministratorController(IAdministratorService administratorService) : Controller
{

    [HttpGet("unblocks")]
    public async Task<IActionResult> UnblockRequestsAsync()
    {
        var user = await administratorService.AllUnblockRequestsAsync();
        return Ok(user);
    }
    [HttpPut("block/user")]
    public async Task<IActionResult> BlockUser([FromForm] BlockedStatusDto blockedStatusDto)
    {
        await administratorService.BlockUserAsync(blockedStatusDto);
        return Ok();
    }
    [HttpPut("block/book")]
    public async Task<IActionResult> BlockBook([FromBody] BlockedStatusDto blockedStatusDto)
    {
        await administratorService.BlockBookAsync(blockedStatusDto);
        return Ok();
    }
    [HttpPut("block/comment")]
    public async Task<IActionResult> BlockComment([FromBody] BlockedStatusDto blockedStatusDto)
    {
        await administratorService.BlockCommentAsync(blockedStatusDto);
        return Ok();
    }
    [HttpPut("unblock/user")]
    public async Task<IActionResult> UnBlockUser([FromForm] Guid id)
    {
        await administratorService.UnBlockUserAsync(id);
        return Ok();
    }
    [HttpPut("unblock/book")]
    public async Task<IActionResult> UnBlockBook([FromBody] Guid id)
    {
        await administratorService.UnBlockBookAsync(id);
        return Ok();
    }
    [HttpPut("unblock/comment")]
    public async Task<IActionResult> UnBlockComment([FromBody] Guid id)
    {
        await administratorService.UnBlockCommentAsync(id);
        return Ok();
    }

}

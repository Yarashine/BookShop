using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Servises.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

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
        var AdministratorId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await administratorService.BlockUserAsync(AdministratorId, blockedStatusDto);
        return Ok();
    }
    [HttpPut("block/book")]
    public async Task<IActionResult> BlockBook([FromBody] BlockedStatusDto blockedStatusDto)
    {
        var AdministratorId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await administratorService.BlockBookAsync(AdministratorId, blockedStatusDto);
        return Ok();
    }
    [HttpPut("block/comment")]
    public async Task<IActionResult> BlockComment([FromBody] BlockedStatusDto blockedStatusDto)
    {
        var AdministratorId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await administratorService.BlockCommentAsync(AdministratorId, blockedStatusDto);
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

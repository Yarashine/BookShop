using Microsoft.AspNetCore.Mvc;
using Models.Dtos;
using Servises.Interfaces;
using Models.Entities;
using System.Runtime.InteropServices;
using Mapster;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("/api")]
public class AdministratorController : Controller
{
    private readonly IAdministratorService administratorService;
    public AdministratorController(IAdministratorService _administratorService)
    {
        administratorService = _administratorService;
    }
    [HttpPut("block/user")]
    public async Task<IActionResult> BlockUser([FromForm] BlockedStatusDto blockedStatusDto)
    {
        bool result = await administratorService.BlockUserAsync(blockedStatusDto);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpPut("block/book")]
    public async Task<IActionResult> BlockBook([FromBody] BlockedStatusDto blockedStatusDto)
    {
        bool result = await administratorService.BlockBookAsync(blockedStatusDto);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpPut("block/comment")]
    public async Task<IActionResult> BlockComment([FromBody] BlockedStatusDto blockedStatusDto)
    {
        bool result = await administratorService.BlockCommentAsync(blockedStatusDto);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpPut("unblock/user")]
    public async Task<IActionResult> UnBlockUser([FromForm] Guid id)
    {
        bool result = await administratorService.UnBlockUserAsync(id);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpPut("unblock/book")]
    public async Task<IActionResult> UnBlockBook([FromBody] Guid id)
    {
        bool result = await administratorService.UnBlockBookAsync(id);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpPut("unblock/comment")]
    public async Task<IActionResult> UnBlockComment([FromBody] Guid id)
    {
        bool result = await administratorService.UnBlockCommentAsync(id);

        if (!result)
            return BadRequest();

        return Ok();
    }

}

using Microsoft.AspNetCore.Mvc;
using Servises.Interfaces;
using Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Api.Controllers;
[ApiController]
[Authorize(Roles = "IsExistedUser")]
[Authorize(Policy = "IsNotBlocked")]
[Route("/api")]
public class CommentController(ICommentService commentService) : Controller
{

    [HttpPost("add/comment")]
    public async Task<IActionResult> Add([FromForm] CommentDto commentDto)
    {
        var AuthorId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await commentService.AddCommentAsync(AuthorId, commentDto);
        return Ok();
    }
    [HttpDelete("delete/comment/{commentId}")]
    public async Task<IActionResult> DeleteLibrary([FromRoute] Guid commentId)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await commentService.DeleteCommentAsync(userId, commentId);
        return Ok();
    }
    [HttpPut("comment/reaction/update")]
    public async Task<IActionResult> AddReaction([FromForm] ReactionDto dto)
    {
        await commentService.UpdateReactionAsync(dto);
        return Ok();
    }
}
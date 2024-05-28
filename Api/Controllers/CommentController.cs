using Microsoft.AspNetCore.Mvc;
using Servises.Interfaces;
using Models.Dtos;
using Servises.Services;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

[ApiController]
[Authorize(Roles = "IsExistedUser")]
[Route("/api")]
public class CommentController(ICommentService commentService) : Controller
{

    [HttpPost("add/comment")]
    public async Task<IActionResult> Add([FromForm] CommentDto commentDto)
    {
        await commentService.AddCommentAsync(commentDto);
        return Ok();
    }
    [HttpDelete("delete/comment")]
    public async Task<IActionResult> DeleteLibrary([FromForm] Guid userId, Guid commentId)
    {
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
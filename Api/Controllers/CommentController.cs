using Microsoft.AspNetCore.Mvc;
using Servises.Interfaces;
using Models.Dtos;

namespace Api.Controllers;

[ApiController]
[Route("/api")]
public class CommentController : Controller
{
    private readonly ICommentService commentService;
    public CommentController(ICommentService _commentService)
    {
        commentService = _commentService;
    }
    /*[HttpGet("{id}")]*/
    /*public async Task<ActionResult<BookDto>> GetById([FromRoute] Guid id)
    {
        var book = await cService.GetByIdAsync(id);

        if (book is null)
            return NotFound();

        return Ok(book);
    }

    [HttpGet("books")]
    public async Task<ActionResult<BookDto>> GetAll()
    {
        var books = await bookService.GetAllAsync();
        return Ok(books);
    }*/

    [HttpPost("add/comment")]
    public async Task<IActionResult> Add([FromForm] CommentDto commentDto)
    {
        bool result = await commentService.AddCommentAsync(commentDto);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpPut("add/comment/reaction")]
    public async Task<IActionResult> AddReaction([FromForm] ReactionDto dto)
    {
        bool result = await commentService.AddReactionToCommentAsync(dto);

        if (!result)
            return BadRequest();

        return Ok();
    }
}
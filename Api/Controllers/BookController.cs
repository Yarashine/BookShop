using Microsoft.AspNetCore.Mvc;
using Servises.Interfaces;
using Models.Dtos;

namespace Api.Controllers;

[ApiController]
[Route("/api")]
public class BookController(IBookService _bookService) : Controller
{
    private readonly IBookService bookService = _bookService;

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetById([FromRoute] Guid id)
    {
        var book = await bookService.GetByIdAsync(id);

        if (book is null)
            return NotFound();

        return Ok(book);
    }

    [HttpGet("books")]
    public async Task<ActionResult<BookDto>> GetAll()
    {
        var books = await bookService.GetAllAsync();
        return Ok(books);
    }

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromForm] BookDto bookDto)
    {
        bool result = await bookService.AddBookAsync(bookDto);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpPut("add/like")]
    public async Task<IActionResult> AddLike([FromForm] Guid userId, Guid bookId )
    {
        bool result = await bookService.AddLikeToBookAsync(userId, bookId);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromForm] Guid bookId, [FromForm] BookDto bookDto)
    {
        bool result = await bookService.UpdateBookAsync(bookId, bookDto);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpGet("download/{id}")]
    public async Task<ActionResult<BookDto>> Download([FromRoute] Guid id)
    {
        var book = await bookService.DownloadBookAsync(id);

        if (book is null)
            return NotFound();

        return Ok(book);
    }
    [HttpPost("buy")]
    public async Task<IActionResult> Buy([FromForm] Guid userId, Guid bookId)
    {
        bool result = await bookService.BuyBookAsync(userId, bookId);

        if (!result)
            return BadRequest();

        return Ok();
    }
    /*[HttpDelete("delete/{id}")]
    public async Task<ActionResult<BookDto>> Delete([FromRoute] Guid userId, Guid bookId)
    {
        var book = await bookService.DeleteBookAsync(userId, bookId);

        if (book is null)
            return NotFound();

        return Ok(book);
    }*/
}
/*Task<bool> AddLikeToBookAsync(Guid userId, Guid bookId);
    Task<bool> BuyBookAsync(Guid userId, Guid bookId);
    Task<EBook> DownloadBookAsync(Guid id);
    //Task<bool> DeleteBookAsync(Guid userId, Guid bookId);
    Task<bool> DeleteLikeFromBookAsync(Guid userId, Guid bookId);
    Task<bool> DeleteFromLibraryAsync(Guid userId, Guid bookId);*/
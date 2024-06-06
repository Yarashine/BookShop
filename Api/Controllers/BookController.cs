using Microsoft.AspNetCore.Mvc;
using Servises.Interfaces;
using Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("/api")]
public class BookController(IBookService _bookService) : Controller
{
    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPut("add/book/{bookId}/co-author/{coAuthorId}")]
    public async Task<IActionResult> AddCoAuthorAsync([FromRoute] Guid bookId, [FromRoute] Guid coAuthorId)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _bookService.AddCoAuthorAsync(bookId, coAuthorId, userId);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPut("remove/book/{bookId}/co-author/{coAuthorId}")]
    public async Task<IActionResult> RemoveCoAuthorAsync([FromRoute] Guid bookId, [FromRoute] Guid coAuthorId)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _bookService.RemoveCoAuthorAsync(bookId, coAuthorId, userId);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPost("add/book/{bookId}/log")]
    public async Task<IActionResult> AddBookChangeLogAsync([FromRoute] Guid bookId, [FromForm] BookChangeLogDto changeLog)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _bookService.AddBookChangeLogAsync(bookId, changeLog, userId);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpGet("book/{bookId}/logs")]
    public async Task<IActionResult> GetBookChangeLogsAsync([FromRoute] Guid bookId)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        var logs = await _bookService.GetBookChangeLogsAsync(userId, bookId);
        return Ok(logs);
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPut("add/book/{bookId}/release/{changeLogId}")]
    public async Task<IActionResult> AddEBookFromChangeLogAsync([FromRoute] Guid bookId, [FromRoute] Guid changeLogId)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _bookService.AddEBookFromChangeLogAsync(bookId, changeLogId, userId);
        return Ok();
    }
    [HttpGet("book/{id}")]
    public async Task<ActionResult<BookInfoDto>> GetById([FromRoute] Guid id)
    {

        var book = await _bookService.GetByIdAsync(id);
        return Ok(book);
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPost("add")]
    public async Task<IActionResult> Add([FromForm] BookDto bookDto)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _bookService.AddBookAsync(userId, bookDto);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPut("add/like/book/{bookId}")]
    public async Task<IActionResult> AddLike([FromRoute] Guid bookId)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _bookService.AddLikeToBookAsync(userId, bookId);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser,IsBlockedUser")]
    [HttpPut("update/book/{bookId}")]
    public async Task<IActionResult> Update([FromRoute] Guid bookId, [FromForm] UpdateBookDto bookDto)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _bookService.UpdateBookAsync(userId, bookId, bookDto);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser,IsBlockedUser")]
    [HttpGet("download/{id}")]
    public async Task<ActionResult> Download([FromRoute] Guid id)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        var book = await _bookService.DownloadBookAsync(userId, id);
        return File(book.Bytes, book.FileType, book.FileName);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> HandleStripeWebhook()
    {
        await _bookService.WebHook(await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(), Request);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPost("buy/{bookId}")]
    public async Task<ActionResult<string>> Buy([FromRoute] Guid bookId)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        string result = await _bookService.BuyBookAsync(userId, bookId);
        return Ok(result);
    }

    [HttpPost("books")]
    public async Task<IActionResult> Get([FromForm] FilterDto filter)
    {
        Guid? userId = null; 
        if (User is not null && User.Identity is not null && User.Identity.IsAuthenticated)
        {
            userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)
                ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        }
        var books = await _bookService.GetBooksAsync(userId, filter);
        return Ok(books);
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPut("delete/like/{bookId}")]
    public async Task<IActionResult> DeleteLike([FromRoute] Guid bookId)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _bookService.DeleteLikeFromBookAsync(userId, bookId);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPut("add/library/{bookId}")]
    public async Task<IActionResult> AddLibrary([FromRoute] Guid bookId)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _bookService.AddToLibraryAsync(userId, bookId);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPut("delete/library/{bookId}")]
    public async Task<IActionResult> DeleteLibrary([FromRoute] Guid bookId)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _bookService.DeleteFromLibraryAsync(userId, bookId);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpDelete("book/delete/{id}")]
    public async Task<ActionResult> DeleteBook([FromRoute] Guid id)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _bookService.DeleteBook(userId, id);
        return Ok();
    }
}
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
    [HttpPut("add/like")]
    public async Task<IActionResult> AddLike([FromForm] Guid bookId)
    {
        var userId = Guid.Parse((User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? throw new BadHttpRequestException("Bad jwt token")).Value);
        await _bookService.AddLikeToBookAsync(userId, bookId);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser,IsBlockedUser")]
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromForm] Guid bookId, [FromForm] UpdateBookDto bookDto)
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
    [HttpPost("buy")]
    public async Task<ActionResult<string>> Buy(Guid bookId)
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
using Microsoft.AspNetCore.Mvc;
using Servises.Interfaces;
using Models.Dtos;
using Servises.Services;
using Stripe;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Models.Entities;
using Microsoft.AspNetCore.Authorization;

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
    public async Task<IActionResult> Add([FromForm] Guid userId, [FromForm] BookDto bookDto)
    {
        await _bookService.AddBookAsync(userId, bookDto);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPut("add/like")]
    public async Task<IActionResult> AddLike([FromForm] Guid userId, [FromForm] Guid bookId)
    {
        await _bookService.AddLikeToBookAsync(userId, bookId);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromForm] Guid userId, [FromForm] Guid bookId, [FromForm] UpdateBookDto bookDto)
    {
        await _bookService.UpdateBookAsync(userId, bookId, bookDto);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [HttpGet("download/{id}")]
    public async Task<ActionResult> Download([FromForm] Guid userId, [FromRoute] Guid id)
    {
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
    public async Task<ActionResult<string>> Buy([FromForm] Guid userId, Guid bookId)
    {
        string result = await _bookService.BuyBookAsync(userId, bookId);
        return Ok(result);
    }

    [HttpPost("books")]
    public async Task<IActionResult> Get([FromForm] FilterDto filter)
    {
        var books = await _bookService.GetBooksAsync(filter);
        return Ok(books);
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPut("delete/like")]
    public async Task<IActionResult> DeleteLike([FromForm] Guid userId, Guid bookId)
    {
        await _bookService.DeleteLikeFromBookAsync(userId, bookId);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPut("add/library")]
    public async Task<IActionResult> AddLibrary([FromForm] Guid userId, Guid bookId)
    {
        await _bookService.AddToLibraryAsync(userId, bookId);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpPut("delete/library")]
    public async Task<IActionResult> DeleteLibrary([FromForm] Guid userId, Guid bookId)
    {
        await _bookService.DeleteFromLibraryAsync(userId, bookId);
        return Ok();
    }

    [Authorize(Roles = "IsExistedUser")]
    [Authorize(Policy = "IsNotBlocked")]
    [HttpDelete("book/delete")]
    public async Task<ActionResult> DeleteBook([FromForm] Guid id, [FromForm] Guid userId)
    {
        await _bookService.DeleteBook(userId, id);
        return Ok();
    }
}
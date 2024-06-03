using Microsoft.AspNetCore.Http;
using Models.Dtos;
using Models.Entities;
using System.Linq.Expressions;

namespace Servises.Interfaces;

public interface IBookService
{
    Task<IReadOnlyList<SummaryBookDto>> GetBooksAsync(Guid? userId, FilterDto filter);
    Task WebHook(string json, HttpRequest request);
    Task AddLikeToBookAsync(Guid userId, Guid bookId);
    Task<string> BuyBookAsync(Guid userId, Guid bookId);
    Task AddBookAsync(Guid userId, BookDto book);
    Task<EBook> DownloadBookAsync(Guid userId, Guid id);
    Task UpdateBookAsync(Guid userId, Guid bookId, UpdateBookDto book);
    Task<BookInfoDto> GetByIdAsync(Guid id);
    Task DeleteLikeFromBookAsync(Guid userId, Guid bookId);
    Task AddToLibraryAsync(Guid userId, Guid bookId);
    Task DeleteFromLibraryAsync(Guid userId, Guid bookId);
    Task DeleteBook(Guid userId, Guid bookId);
}

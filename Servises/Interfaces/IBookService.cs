using Models.Dtos;
using Models.Entities;

namespace Servises.Interfaces;

public interface IBookService
{

    Task<bool> AddLikeToBookAsync(Guid userId, Guid bookId);
    Task<bool> BuyBookAsync(Guid userId, Guid bookId);
    Task<bool> AddBookAsync(BookDto book);
    Task<EBook> DownloadBookAsync(Guid id);
    //Task<bool> DeleteBookAsync(Guid userId, Guid bookId);
    Task<bool> UpdateBookAsync(Guid bookId, BookDto book);
    Task<IReadOnlyList<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(Guid id);
    Task<bool> DeleteLikeFromBookAsync(Guid userId, Guid bookId);
    Task<bool> AddToLibraryAsync(Guid userId, Guid bookId);
    Task<bool> DeleteFromLibraryAsync(Guid userId, Guid bookId);
}

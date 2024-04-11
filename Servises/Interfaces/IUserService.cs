using Models.Entities;
using Models.Dtos;

namespace Servises.Interfaces;

public interface IUserService
{

    Task<bool> UpdateUserAsync(RegisterUserDto user);
    Task<IReadOnlyList<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User?> GetByIdWithComments(Guid id);
    Task<List<Book>?> GetFavoritesAsync(Guid id);
    Task<List<Book>?> GetLibraryAsync(Guid id);
    Task<List<Book>?> GetBookToSellAsync(Guid id);
    Task<List<Book>?> GetPurchasedBookAsync(Guid id);



}

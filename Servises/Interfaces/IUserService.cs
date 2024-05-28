using Models.Entities;
using Models.Dtos;

namespace Servises.Interfaces;

public interface IUserService
{
    Task UnbanRequestAsync(Guid userId, string? description);
    Task UpdateUserAsync(Guid userId, UpdateUserDto user);
    Task DeleteUserAsync(Guid id);
    Task<User> GetByIdAsync(Guid id);
    Task<List<SummaryBookDto>> GetFavoritesAsync(Guid id);
    Task<List<SummaryBookDto>> GetLibraryAsync(Guid id);
    Task<List<SummaryBookDto>> GetBookToSellAsync(Guid id);
    Task<List<SummaryBookDto>> GetPurchasedBookAsync(Guid id);
}

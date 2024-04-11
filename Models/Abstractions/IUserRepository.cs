using Models.Entities;
using System.Linq.Expressions;

namespace Models.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithCommentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithStatusAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithBooksToSellAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithFavoritesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithLibraryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByIdWithPurchasedBooksAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> ListAsync(Expression<Func<User, bool>> filter, CancellationToken cancellationToken = default,
    params Expression<Func<User, object>>[]? includesProperties);
    Task AddAsync(User entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> filter, CancellationToken cancellationToken = default);


}

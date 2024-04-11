using Models.Entities;
using System.Linq.Expressions;

namespace Models.Abstractions;

public interface IBookRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Book?> GetByIdWithBookAndMediaAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Book?> GetByIdWithBookAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Book?> GetByIdWithMediaAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Book>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Book>> ListAsync(Expression<Func<Book, bool>> filter, CancellationToken cancellationToken = default,
    params Expression<Func<Book, object>>[]? includesProperties);
    Task AddAsync(Book entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Book?> FirstOrDefaultAsync(Expression<Func<Book, bool>> filter, CancellationToken cancellationToken = default);
    Task UpdateAsync(Book entity, CancellationToken cancellationToken = default);

}

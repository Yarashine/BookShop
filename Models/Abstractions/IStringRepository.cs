
using System.Linq.Expressions;
using Models.Entities;

namespace Models.Abstractions;

public interface IStringRepository<T> where T : StringEntity
{
    Task<T?> GetByNameAsync(string name, CancellationToken cancellationToken = default,
    params Expression<Func<T, object>>[]? includesProperties);
    Task<T?> GetByNameWithBooksAsync(string name, CancellationToken cancellationToken = default,
    params Expression<Func<T, object>>[]? includesProperties);
    Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default,
    params Expression<Func<T, object>>[]? includesProperties);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(string name, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);

}

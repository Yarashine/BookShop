using System.Linq.Expressions;

namespace Models.Abstractions;

public interface IRepository<T> 
{
    Task DetacheAsync(T entity);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default,
    params Expression<Func<T, object>>[]? includesProperties);
    Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default,
    params Expression<Func<T, object>>[]? includesProperties);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default);


}

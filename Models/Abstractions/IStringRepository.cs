
using System.Linq.Expressions;
using Models.Entities;

namespace Models.Abstractions;

public interface IStringRepository
{
    Task<StringEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default,
    params Expression<Func<StringEntity, object>>[]? includesProperties);
    Task<StringEntity?> GetByNameWithBooksAsync(string name, CancellationToken cancellationToken = default,
    params Expression<Func<StringEntity, object>>[]? includesProperties);
    Task<IReadOnlyList<StringEntity>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StringEntity>> ListAsync(Expression<Func<StringEntity, bool>> filter, CancellationToken cancellationToken = default,
    params Expression<Func<StringEntity, object>>[]? includesProperties);
    Task AddAsync(StringEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(string name, CancellationToken cancellationToken = default);
    Task<StringEntity?> FirstOrDefaultAsync(Expression<Func<StringEntity, bool>> filter, CancellationToken cancellationToken = default);

}

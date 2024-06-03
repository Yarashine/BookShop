using Models.Entities;
using System.Linq.Expressions;

namespace Models.Abstractions;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Comment>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Comment>> ListAsync(Expression<Func<Comment, bool>> filter, CancellationToken cancellationToken = default,
    params Expression<Func<Comment, object>>[]? includesProperties);
    Task AddAsync(Comment entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Comment?> FirstOrDefaultAsync(Expression<Func<Comment, bool>> filter, CancellationToken cancellationToken = default);
    Task UpdateAsync(Comment entity, CancellationToken cancellationToken = default);

}

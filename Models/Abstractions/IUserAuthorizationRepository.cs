using System.Linq.Expressions;
using Models.Entities;

namespace Models.Abstractions;

public interface IUserAuthorizationRepository
{
    Task<UserAuthorizationInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default,
    params Expression<Func<UserAuthorizationInfo, object>>[]? includesProperties);
    Task<IReadOnlyList<UserAuthorizationInfo>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<UserAuthorizationInfo>> ListAsync(Expression<Func<UserAuthorizationInfo, bool>> filter, CancellationToken cancellationToken = default,
    params Expression<Func<UserAuthorizationInfo, object>>[]? includesProperties);
    Task AddAsync(UserAuthorizationInfo entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);
    Task<UserAuthorizationInfo?> FirstOrDefaultAsync(Expression<Func<UserAuthorizationInfo, bool>> filter, CancellationToken cancellationToken = default);

}
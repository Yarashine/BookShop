using System.Linq.Expressions;
using Models.Entities;

namespace Models.Abstractions;

public interface IAdminAuthorizationRepository
{
    Task<AdminAuthorizationInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default,
    params Expression<Func<AdminAuthorizationInfo, object>>[]? includesProperties);
    Task<IReadOnlyList<AdminAuthorizationInfo>> ListAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AdminAuthorizationInfo>> ListAsync(Expression<Func<AdminAuthorizationInfo, bool>> filter, CancellationToken cancellationToken = default,
    params Expression<Func<AdminAuthorizationInfo, object>>[]? includesProperties);
    Task AddAsync(AdminAuthorizationInfo entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);
    Task<AdminAuthorizationInfo?> FirstOrDefaultAsync(Expression<Func<AdminAuthorizationInfo, bool>> filter, CancellationToken cancellationToken = default);

}
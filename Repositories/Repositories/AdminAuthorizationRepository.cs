using Microsoft.EntityFrameworkCore;
using Models.Abstractions;
using Models.Entities;
using System.Linq.Expressions;

namespace Repositories.Repositories;

public class AdminAuthorizationRepository : IAdminAuthorizationRepository
{
    protected readonly ShopContext context;
    protected readonly DbSet<AdminAuthorizationInfo> entities;
    public AdminAuthorizationRepository(ShopContext _context)
    {
        context = _context;
        entities = _context.Set<AdminAuthorizationInfo>();
    }
    public async Task AddAsync(AdminAuthorizationInfo entity, CancellationToken cancellationToken)
    {
        await entities.AddAsync(entity);
    }

    public async Task DeleteAsync(string email, CancellationToken cancellationToken)
    {
        AdminAuthorizationInfo? entity = await FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
        if (entity is not null)
            entities.Remove(entity);
    }

    public async Task<AdminAuthorizationInfo?> FirstOrDefaultAsync(Expression<Func<AdminAuthorizationInfo, bool>> filter, CancellationToken cancellationToken)
    {
        return await entities.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<AdminAuthorizationInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default,
        params Expression<Func<AdminAuthorizationInfo, object>>[]? includesProperties)
    {
        IQueryable<AdminAuthorizationInfo>? query = entities.AsQueryable();
        if (includesProperties is not null && includesProperties.Any())
        {
            foreach (Expression<Func<AdminAuthorizationInfo, object>>? included in includesProperties)
            {
                query = query.Include(included);
            }
        }
        return await query.FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
    }

    public async Task<IReadOnlyList<AdminAuthorizationInfo>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await entities.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AdminAuthorizationInfo>> ListAsync(Expression<Func<AdminAuthorizationInfo, bool>>? filter,
    CancellationToken cancellationToken = default, params Expression<Func<AdminAuthorizationInfo, object>>[]? includesProperties)
    {
        IQueryable<AdminAuthorizationInfo>? query = entities.AsQueryable();
        if (includesProperties is not null && includesProperties.Any())
        {
            foreach (Expression<Func<AdminAuthorizationInfo, object>>? included in includesProperties)
            {
                query = query.Include(included);
            }
        }
        if (filter != null)
        {
            query = query.Where(filter);
        }
        return await query.ToListAsync();
    }
    public Task UpdateAsync(AdminAuthorizationInfo entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}


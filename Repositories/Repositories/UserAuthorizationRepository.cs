using Microsoft.EntityFrameworkCore;
using Models.Abstractions;
using Models.Entities;
using System.Linq.Expressions;

namespace Repositories.Repositories;

public class UserAuthorizationRepository : IUserAuthorizationRepository
{
    protected readonly ShopContext context;
    protected readonly DbSet<UserAuthorizationInfo> entities;
    public UserAuthorizationRepository(ShopContext _context)
    {
        context = _context;
        entities = _context.Set<UserAuthorizationInfo>();
    }
    public async Task AddAsync(UserAuthorizationInfo entity, CancellationToken cancellationToken)
    {
        await entities.AddAsync(entity);
    }

    public async Task DeleteAsync(string email, CancellationToken cancellationToken)
    {
        UserAuthorizationInfo? entity = await FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
        if (entity is not null)
            entities.Remove(entity);
    }

    public async Task<UserAuthorizationInfo?> FirstOrDefaultAsync(Expression<Func<UserAuthorizationInfo, bool>> filter, CancellationToken cancellationToken)
    {
        return await entities.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<UserAuthorizationInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default,
        params Expression<Func<UserAuthorizationInfo, object>>[]? includesProperties)
    {
        IQueryable<UserAuthorizationInfo>? query = entities.AsQueryable();
        if (includesProperties is not null && includesProperties.Any())
        {
            foreach (Expression<Func<UserAuthorizationInfo, object>>? included in includesProperties)
            {
                query = query.Include(included);
            }
        }
        return await query.FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
    }

    public async Task<IReadOnlyList<UserAuthorizationInfo>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await entities.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserAuthorizationInfo>> ListAsync(Expression<Func<UserAuthorizationInfo, bool>>? filter,
    CancellationToken cancellationToken = default, params Expression<Func<UserAuthorizationInfo, object>>[]? includesProperties)
    {
        IQueryable<UserAuthorizationInfo>? query = entities.AsQueryable();
        if (includesProperties is not null && includesProperties.Any())
        {
            foreach (Expression<Func<UserAuthorizationInfo, object>>? included in includesProperties)
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
    public Task UpdateAsync(UserAuthorizationInfo entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}


using Microsoft.EntityFrameworkCore;
using Models.Abstractions;
using Models.Entities;
using System.Linq.Expressions;

namespace Repositories.Repositories;

public class EfRepository<T>(ShopContext _context) : IRepository<T> where T : Entity
{
    protected readonly ShopContext context = _context;
    protected readonly DbSet<T> entities = _context.Set<T>();

    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await entities.AddAsync(entity, cancellationToken);
    }
    public async Task DetacheAsync(T entity)
    {
        context.Entry(entity).State = EntityState.Detached;
    }
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        T? entity = await FirstOrDefaultAsync(e => e.Id==id, cancellationToken);
        if(entity is not null) 
        entities.Remove(entity);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
    {
        return await entities.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[]? includesProperties)
    {
        IQueryable<T>? query = entities.AsQueryable();
        if (includesProperties is not null && includesProperties.Length != 0)
        {
            foreach (Expression<Func<T, object>>? included in includesProperties)
            {
                query = query.Include(included);
            }
        }
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await entities.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? filter,
    CancellationToken cancellationToken = default, params Expression<Func<T, object>>[]? includesProperties)
    {
        IQueryable<T>? query = entities.AsQueryable();
        if (includesProperties is not null && includesProperties.Length != 0)
        {
            foreach (Expression<Func<T, object>>? included in includesProperties)
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

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}

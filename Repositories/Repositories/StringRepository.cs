using Microsoft.EntityFrameworkCore;
using Models.Abstractions;
using Models.Entities;
using System.Linq.Expressions;

namespace Repositories.Repositories;

public class StringRepository<T> : IStringRepository<T> where T : StringEntity
{
    protected readonly ShopContext context;
    protected readonly DbSet<T> entities;
    public StringRepository(ShopContext _context)
    {
        context = _context;
        entities = _context.Set<T>();
    }
    public async Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await entities.AddAsync(entity);
    }

    public async Task DeleteAsync(string name, CancellationToken cancellationToken)
    {
        T? entity = await FirstOrDefaultAsync(e => e.Name == name, cancellationToken);
        if (entity is not null)
        entities.Remove(entity);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
    {
        return await entities.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<T?> GetByNameAsync(string name, CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[]? includesProperties)
    {
        IQueryable<T>? query = entities.AsQueryable();
        if (includesProperties is not null && includesProperties.Any())
        {
            foreach (Expression<Func<T, object>>? included in includesProperties)
            {
                query = query.Include(included);
            }
        }
        return await query.FirstOrDefaultAsync(e => e.Name == name, cancellationToken);
    }

    public async Task<T?> GetByNameWithBooksAsync(string name, CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[]? includesProperties)
    {
        IQueryable<T>? query = entities.Include(s => s.Books).AsQueryable();
        if (includesProperties is not null && includesProperties.Any())
        {
            foreach (Expression<Func<T, object>>? included in includesProperties)
            {
                query = query.Include(included);
            }
        }
        return await query.FirstOrDefaultAsync(e => e.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await entities.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? filter,
    CancellationToken cancellationToken = default, params Expression<Func<T, object>>[]? includesProperties)
    {
        IQueryable<T>? query = entities.AsQueryable();
        if (includesProperties is not null && includesProperties.Any())
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
}


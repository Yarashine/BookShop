using Microsoft.EntityFrameworkCore;
using Models.Abstractions;
using Models.Entities;
using System.Linq.Expressions;

namespace Repositories.Repositories;

public class StringRepository : IStringRepository
{
    protected readonly ShopContext context;
    protected readonly DbSet<StringEntity> entities;
    public StringRepository(ShopContext _context)
    {
        context = _context;
        entities = _context.Set<StringEntity>();
    }
    public async Task AddAsync(StringEntity entity, CancellationToken cancellationToken)
    {
        await entities.AddAsync(entity);
    }

    public async Task DeleteAsync(string name, CancellationToken cancellationToken)
    {
        StringEntity? entity = await FirstOrDefaultAsync(e => e.Name == name, cancellationToken);
        if (entity is not null)
        entities.Remove(entity);
    }

    public async Task<StringEntity?> FirstOrDefaultAsync(Expression<Func<StringEntity, bool>> filter, CancellationToken cancellationToken)
    {
        return await entities.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<StringEntity?> GetByNameAsync(string name, CancellationToken cancellationToken = default,
        params Expression<Func<StringEntity, object>>[]? includesProperties)
    {
        IQueryable<StringEntity>? query = entities.AsQueryable();
        if (includesProperties is not null && includesProperties.Any())
        {
            foreach (Expression<Func<StringEntity, object>>? included in includesProperties)
            {
                query = query.Include(included);
            }
        }
        return await query.FirstOrDefaultAsync(e => e.Name == name, cancellationToken);
    }

    public async Task<StringEntity?> GetByNameWithBooksAsync(string name, CancellationToken cancellationToken = default,
        params Expression<Func<StringEntity, object>>[]? includesProperties)
    {
        IQueryable<StringEntity>? query = entities.Include(s => s.Books).AsQueryable();
        if (includesProperties is not null && includesProperties.Any())
        {
            foreach (Expression<Func<StringEntity, object>>? included in includesProperties)
            {
                query = query.Include(included);
            }
        }
        return await query.FirstOrDefaultAsync(e => e.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<StringEntity>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await entities.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StringEntity>> ListAsync(Expression<Func<StringEntity, bool>>? filter,
    CancellationToken cancellationToken = default, params Expression<Func<StringEntity, object>>[]? includesProperties)
    {
        IQueryable<StringEntity>? query = entities.AsQueryable();
        if (includesProperties is not null && includesProperties.Any())
        {
            foreach (Expression<Func<StringEntity, object>>? included in includesProperties)
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


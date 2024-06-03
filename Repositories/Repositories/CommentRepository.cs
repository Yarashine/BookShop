using Microsoft.EntityFrameworkCore;
using Models.Abstractions;
using Models.Entities;
using System.Linq.Expressions;

namespace Repositories.Repositories;

public class CommentRepository(ShopContext _context) : ICommentRepository
{
    protected readonly ShopContext context = _context;
    protected readonly DbSet<Comment> entities = _context.Set<Comment>();

    public async Task AddAsync(Comment entity, CancellationToken cancellationToken)
    {
        await entities.AddAsync(entity, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        Comment? entity = await FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity is not null)
            entities.Remove(entity);
    }

    public async Task<Comment?> FirstOrDefaultAsync(Expression<Func<Comment, bool>> filter, CancellationToken cancellationToken)
    {
        return await entities.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        IQueryable<Comment>? query = entities.Include(c => c.Status).AsQueryable();
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    public async Task<IReadOnlyList<Comment>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await entities.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Comment>> ListAsync(Expression<Func<Comment, bool>>? filter,
    CancellationToken cancellationToken = default, params Expression<Func<Comment, object>>[]? includesProperties)
    {
        IQueryable<Comment>? query = entities.AsQueryable();
        if (includesProperties is not null && includesProperties.Length != 0)
        {
            foreach (Expression<Func<Comment, object>>? included in includesProperties)
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

    public Task UpdateAsync(Comment entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

}

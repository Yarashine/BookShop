using Microsoft.EntityFrameworkCore;
using Models.Abstractions;
using Models.Entities;
using System.Linq.Expressions;

namespace Repositories.Repositories;

public class BookRepository : IBookRepository
{
    protected readonly ShopContext context;
    protected readonly DbSet<Book> entities;
    public BookRepository(ShopContext _context)
    {
        context = _context;
        entities = _context.Set<Book>();
    }
    public async Task AddAsync(Book entity, CancellationToken cancellationToken)
    {
        await entities.AddAsync(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        Book? entity = await FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (entity is not null)
            entities.Remove(entity);
    }

    public async Task<Book?> FirstOrDefaultAsync(Expression<Func<Book, bool>> filter, CancellationToken cancellationToken)
    {
        return await entities.FirstOrDefaultAsync(filter, cancellationToken);
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        IQueryable<Book>? query = entities.AsQueryable();
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    public async Task<Book?> GetByIdWithBookAsync(Guid id, CancellationToken cancellationToken = default)
    {
        IQueryable<Book>? query = entities.Include(b => b.EBook).AsQueryable();
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    public async Task<Book?> GetByIdWithAllAsync(Guid id, CancellationToken cancellationToken = default)
    {
        IQueryable<Book>? query = entities.Include(b => b.Cover)
            .Include(b => b.Genres).Include(b => b.Tags)
            .Include(b => b.Comments).AsQueryable();
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Book>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await entities.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Book>> ListAsync(Expression<Func<Book, bool>>? filter,
    CancellationToken cancellationToken = default, params Expression<Func<Book, object>>[]? includesProperties)
    {
        IQueryable<Book>? query = entities.Include(b => b.Cover)
            .Include(b => b.Genres).Include(b => b.Tags).AsQueryable();
        if (includesProperties is not null && includesProperties.Any())
        {
            foreach (Expression<Func<Book, object>>? included in includesProperties)
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

    public Task UpdateAsync(Book entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

}

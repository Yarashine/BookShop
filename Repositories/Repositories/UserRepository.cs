using Microsoft.EntityFrameworkCore;
using Models.Abstractions;
using Models.Entities;
using System.Linq.Expressions;

namespace Repositories.Repositories;

public class UserRepository : IUserRepository
{
    protected readonly ShopContext context;
    protected readonly DbSet<User> entities;
    public UserRepository(ShopContext _context)
    {
        context = _context;
        entities = _context.Set<User>();
    }
    public async Task AddAsync(User entity, CancellationToken cancellationToken)
    {
        await entities.AddAsync(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        User? entity = await FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if(entity is not null) 
        entities.Remove(entity);
    }

    public async Task<User?> FirstOrDefaultAsync(Expression<Func<User, bool>> filter, CancellationToken cancellationToken)
    {
        return await entities.Include(u => u.BooksToSell).FirstOrDefaultAsync(cancellationToken);
    }
    public async Task<User?> GetByIdWithStatusAsync(Guid id, CancellationToken cancellationToken = default)
    {
        IQueryable<User>? query = entities.Include(u => u.Status).AsQueryable();
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    public async Task<User?> GetByIdWithCommentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        IQueryable<User>? query = entities.Include(u => u.Comments).AsQueryable();
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        IQueryable<User>? query = entities.AsQueryable();
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    public async Task<User?> GetByIdWithBooksToSellAsync(Guid id, CancellationToken cancellationToken = default)
    {
        IQueryable<User>? query = entities.Include(u => u.BooksToSell).AsQueryable();
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    public async Task<User?> GetByIdWithFavoritesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        IQueryable<User>? query = entities.Include(u => u.Favorites).AsQueryable();
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    public async Task<User?> GetByIdWithLibraryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        IQueryable<User>? query = entities.Include(u => u.Library).AsQueryable();
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    public async Task<User?> GetByIdWithPurchasedBooksAsync(Guid id, CancellationToken cancellationToken = default)
    {
        IQueryable<User>? query = entities.Include(u => u.PurchasedBooks).AsQueryable();
        return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
    public async Task<IReadOnlyList<User>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await entities.Include(u => u.BooksToSell).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<User>> ListAsync(Expression<Func<User, bool>>? filter,
    CancellationToken cancellationToken = default, params Expression<Func<User, object>>[]? includesProperties)
    {
        IQueryable<User>? query = entities.Include(u => u.BooksToSell).AsQueryable();
        if (includesProperties is not null && includesProperties.Any())
        {
            foreach (Expression<Func<User, object>>? included in includesProperties)
            {
                if (included != null)
                {
                    query = query.Include(included);
                }
            }
        }
        if (filter != null)
        {
            query = query.Where(filter);
        }
        return await query.ToListAsync();
    }

    public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}

using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Models.Abstractions;

namespace Repositories.Repositories
{
    public class BookRepository(ShopContext context) : IBookRepository
    {
        private readonly ShopContext _context = context;
        private readonly DbSet<Book> _entities = context.Set<Book>();

        public async Task<Book?> GetByIdWithCoAuthorsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            IQueryable<Book>? query = _entities.Include(b => b.CoAuthors).AsQueryable();
            return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }
        public async Task AddAsync(Book entity, CancellationToken cancellationToken)
        {
            await _entities.AddAsync(entity, cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            Book? entity = await FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (entity is not null)
                _entities.Remove(entity);
        }

        public async Task<Book?> FirstOrDefaultAsync(Expression<Func<Book, bool>> filter, CancellationToken cancellationToken)
        {
            return await _entities.FirstOrDefaultAsync(filter, cancellationToken);
        }

        public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            IQueryable<Book> query = _entities.AsQueryable();
            return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<Book?> GetByIdWithBookAsync(Guid id, CancellationToken cancellationToken = default)
        {
            IQueryable<Book> query = _entities.Include(b => b.EBooks).AsQueryable();
            return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<Book?> GetByIdWithAllAsync(Guid id, CancellationToken cancellationToken = default)
        {
            IQueryable<Book> query = _entities.Include(b => b.Cover)
                .Include(b => b.Genres).Include(b => b.Tags)
                .Include(b => b.Comments)
                .Include(b => b.CoAuthors)
                .Include(b => b.ChangeLogs).ThenInclude(cl => cl.CreatedBy)
                .Include(b => b.ChangeLogs).ThenInclude(cl => cl.EBook)
                .AsQueryable();
            return await query.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Book>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            return await _entities.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Book>> ListAsync(Expression<Func<Book, bool>>? filter, CancellationToken cancellationToken = default, params Expression<Func<Book, object>>[]? includesProperties)
        {
            IQueryable<Book> query = _entities.Include(b => b.Cover)
                .Include(b => b.Genres).Include(b => b.Tags).AsQueryable();
            if (includesProperties is not null && includesProperties.Length != 0)
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
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }
    }
}

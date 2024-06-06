using Models.Abstractions;
using Models.Entities;
using Repositories.Repositories;

namespace Repositories;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly ShopContext context;
    private readonly Lazy<IUserRepository> userRepository;
    private readonly Lazy<IBookRepository> bookRepository;
    private readonly Lazy<ICommentRepository> commentRepository;
    private readonly Lazy<IRepository<UnbanRequest>> unbanRequestRepository;
    private readonly Lazy<IRepository<Administrator>> administratorRepository;
    private readonly Lazy<IRepository<EBook>> eBookRepository;
    private readonly Lazy<IRepository<BookChangeLog>> bookChangeLogRepository;
    private readonly Lazy<IStringRepository<Genre>> genreRepository;
    private readonly Lazy<IStringRepository<Tag>> tagRepository;
    private readonly Lazy<IUserAuthorizationRepository> userAuthorizationRepository;
    private readonly Lazy<IAdminAuthorizationRepository> adminAuthorizationRepository;

    public EfUnitOfWork(ShopContext _context)
    {
        context = _context;
        userRepository = new Lazy<IUserRepository>(() => new UserRepository(context));
        bookRepository = new Lazy<IBookRepository>(() => new BookRepository(context));
        unbanRequestRepository = new Lazy<IRepository<UnbanRequest>>(() => new EfRepository<UnbanRequest>(context));
        commentRepository = new Lazy<ICommentRepository>(() => new CommentRepository(context));
        administratorRepository = new Lazy<IRepository<Administrator>>(() => new EfRepository<Administrator>(context));
        eBookRepository = new Lazy<IRepository<EBook>>(() => new EfRepository<EBook>(context));
        bookChangeLogRepository = new Lazy<IRepository<BookChangeLog>>(() => new EfRepository<BookChangeLog>(context));
        genreRepository = new Lazy<IStringRepository<Genre>>(() => new StringRepository<Genre>(context));
        tagRepository = new Lazy<IStringRepository<Tag>>(() => new StringRepository<Tag>(context));
        userAuthorizationRepository = new Lazy<IUserAuthorizationRepository>(() => new UserAuthorizationRepository(context));
        adminAuthorizationRepository = new Lazy<IAdminAuthorizationRepository>(() => new AdminAuthorizationRepository(context));

    }
    public IUserRepository UserRepository => userRepository.Value;
    public IBookRepository BookRepository => bookRepository.Value;
    public IRepository<UnbanRequest> UnbanRequestRepository => unbanRequestRepository.Value;
    public ICommentRepository CommentRepository => commentRepository.Value;
    public IRepository<Administrator> AdministratorRepository => administratorRepository.Value;
    public IRepository<EBook> EBookRepository => eBookRepository.Value;
    public IRepository<BookChangeLog> BookChangeLogRepository => bookChangeLogRepository.Value;
    public IStringRepository<Genre> GenreRepository => genreRepository.Value;
    public IStringRepository<Tag> TagRepository => tagRepository.Value;
    public IUserAuthorizationRepository UserAuthorizationRepository => userAuthorizationRepository.Value;
    public IAdminAuthorizationRepository AdminAuthorizationRepository => adminAuthorizationRepository.Value;
    public async Task CreateDataBaseAsync() => await context.Database.EnsureCreatedAsync();
    public async Task SaveAllAsync() => await context.SaveChangesAsync();
}

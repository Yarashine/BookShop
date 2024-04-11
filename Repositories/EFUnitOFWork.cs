using Models.Abstractions;
using Models.Entities;
using Repositories.Repositories;

namespace Repositories;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly ShopContext context;
    private readonly Lazy<IUserRepository> userRepository;

    private readonly Lazy<IBookRepository> bookRepository;
    private readonly Lazy<IRepository<Comment>> commentRepository;
    private readonly Lazy<IRepository<Administrator>> administratorRepository;
    private readonly Lazy<IStringRepository> genreRepository;
    private readonly Lazy<IStringRepository> tagRepository;
    private readonly Lazy<IUserAuthorizationRepository> userAuthorizationRepository;
    private readonly Lazy<IAdminAuthorizationRepository> adminAuthorizationRepository;

    public EfUnitOfWork(ShopContext _context)
    {
        context = _context;
        userRepository = new Lazy<IUserRepository>(() => new UserRepository(context));
        bookRepository = new Lazy<IBookRepository>(() => new BookRepository(context));
        commentRepository = new Lazy<IRepository<Comment>>(() => new EfRepository<Comment>(context));
        administratorRepository = new Lazy<IRepository<Administrator>>(() => new EfRepository<Administrator>(context));
        genreRepository = new Lazy<IStringRepository>(() => new StringRepository(context));
        tagRepository = new Lazy<IStringRepository>(() => new StringRepository(context));
        userAuthorizationRepository = new Lazy<IUserAuthorizationRepository>(() => new UserAuthorizationRepository(context));
        adminAuthorizationRepository = new Lazy<IAdminAuthorizationRepository>(() => new AdminAuthorizationRepository(context));

    }
    public IUserRepository UserRepository => userRepository.Value;

    public IBookRepository BookRepository => bookRepository.Value;
    public IRepository<Comment> CommentRepository => commentRepository.Value;
    public IRepository<Administrator> AdministratorRepository => administratorRepository.Value;
    public IStringRepository GenreRepository => genreRepository.Value;
    public IStringRepository TagRepository => tagRepository.Value;
    public IUserAuthorizationRepository UserAuthorizationRepository => userAuthorizationRepository.Value;
    public IAdminAuthorizationRepository AdminAuthorizationRepository => adminAuthorizationRepository.Value;
    public async Task CreateDataBaseAsync() => await context.Database.EnsureCreatedAsync();
    public async Task SaveAllAsync() => await context.SaveChangesAsync();
}

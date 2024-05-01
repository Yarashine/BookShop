using Models.Entities;

namespace Models.Abstractions;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IBookRepository BookRepository { get; }
    IRepository<Comment> CommentRepository { get; }
    IRepository<Administrator> AdministratorRepository { get; }
    IStringRepository<Tag> TagRepository { get; }
    IStringRepository<Genre> GenreRepository { get; }
    IUserAuthorizationRepository UserAuthorizationRepository { get; }
    IAdminAuthorizationRepository AdminAuthorizationRepository { get; }
    public Task SaveAllAsync();
    // public Task DeleteDataBaseAsync();
    public Task CreateDataBaseAsync();
}

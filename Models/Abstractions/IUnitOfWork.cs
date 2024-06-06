using Models.Entities;

namespace Models.Abstractions;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IBookRepository BookRepository { get; }
    IRepository<UnbanRequest> UnbanRequestRepository { get; }
    ICommentRepository CommentRepository { get; }
    IRepository<Administrator> AdministratorRepository { get; }
    IRepository<BookChangeLog> BookChangeLogRepository { get; }
    IRepository<EBook> EBookRepository { get; }
    IStringRepository<Tag> TagRepository { get; }
    IStringRepository<Genre> GenreRepository { get; }
    IUserAuthorizationRepository UserAuthorizationRepository { get; }
    IAdminAuthorizationRepository AdminAuthorizationRepository { get; }
    public Task SaveAllAsync();
    public Task CreateDataBaseAsync();
}

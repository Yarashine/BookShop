using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.Reflection;

namespace Repositories;

public partial class ShopContext(DbContextOptions<ShopContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Administrator> Administrators { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<UserStatus> UserStatus { get; set; }
    public DbSet<CommentStatus> CommentStatus { get; set; }
    public DbSet<BookStatus> BookStatus { get; set; }
    public DbSet<BookImage> BookImages { get; set; }
    public DbSet<UserImage> UserImages { get; set; }
    public DbSet<EBook> EBooks { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<AdminAuthorizationInfo> AdminAuthorizations { get; set; }
    public DbSet<UserAuthorizationInfo> UserAuthorizations { get; set; }
    public DbSet<UnbanRequest> UnbanRequests { get; set; }
    public DbSet<BookChangeLog> BookChangeLogs { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<StateType>();
        modelBuilder.HasPostgresEnum<BlockedType>();

        modelBuilder.Ignore<Entity>();
        modelBuilder.Ignore<StringEntity>();
        modelBuilder.Ignore<AuthorizationInfo>();
        modelBuilder.Ignore<IfBlockedStatus>();


        modelBuilder.Entity<User>()
            .HasMany(a => a.BooksToSell)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<User>()
            .HasMany(u => u.Favorites)
            .WithMany(b => b.AddedInFavorites);
        modelBuilder.Entity<User>()
            .HasMany(u => u.Library)
            .WithMany(b => b.AddedInLibrary);
        modelBuilder.Entity<User>()
            .HasMany(u => u.PurchasedBooks)
            .WithMany(b => b.BoughtBooks);
        modelBuilder.Entity<User>()
            .HasMany(u => u.Comments)
            .WithOne(c => c.Author)
            .HasForeignKey(c => c.AuthorId);
        modelBuilder.Entity<User>()
            .HasMany(u => u.Reactions)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Status)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.UserImage)
            .WithOne(i => i.User)
            .HasForeignKey<UserImage>(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.AuthorizationInfo)
            .WithOne(i => i.User)
            .HasForeignKey<UserAuthorizationInfo>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserAuthorizationInfo>().HasKey(e => e.Email);
        modelBuilder.Entity<AdminAuthorizationInfo>().HasKey(e => e.Email);

        modelBuilder.Entity<Tag>().HasKey(s => s.Name);
        modelBuilder.Entity<Genre>().HasKey(s => s.Name);
        


        modelBuilder.Entity<Book>()
            .HasMany(b => b.Comments)
            .WithOne(c => c.Book)
            .HasForeignKey(c => c.BookId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Tags)
            .WithMany(t => t.Books);
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Genres)
            .WithMany(g => g.Books);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.EBooks)
            .WithOne(e => e.Book)
            .HasForeignKey(e => e.BookId);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.CoAuthors)
            .WithMany(u => u.CoAuthoredBooks)
            .UsingEntity(j => j.ToTable("BookCoAuthors"));

        modelBuilder.Entity<Book>()
            .HasMany(b => b.ChangeLogs)
            .WithOne(cl => cl.Book)
            .HasForeignKey(cl => cl.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookChangeLog>()
            .HasOne(cl => cl.CreatedBy)
            .WithMany(u => u.BookChangeLogs)
            .HasForeignKey(cl => cl.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);  

        modelBuilder.Entity<BookChangeLog>()
            .HasOne(cl => cl.EBook)
            .WithOne(eb => eb.ChangeLog)
            .HasForeignKey<BookChangeLog>(cl => cl.EBookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Book>()
            .HasMany(b => b.Status)
            .WithOne(s => s.Book)
            .HasForeignKey(s=>s.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Cover)
            .WithOne(i => i.Book)
            .HasForeignKey<BookImage>(i=>i.BookId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Comment>()
            .HasMany(c => c.Reactions)
            .WithOne(r => r.Comment)
            .HasForeignKey(r => r.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Status)
            .WithOne(s => s.Comment)
            .HasForeignKey<CommentStatus>(s => s.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UnbanRequest>()
            .HasOne(u => u.User)
            .WithOne()
            .HasForeignKey<UnbanRequest>(u => u.UserId);
        modelBuilder.Entity<UnbanRequest>()
            .HasOne(u => u.Status)
            .WithOne()
            .HasForeignKey<UnbanRequest>(u => u.StatusId);
            

        modelBuilder.Entity<Administrator>()
            .HasOne(a => a.AuthorizationInfo)
            .WithOne(i => i.User)
            .HasForeignKey<AdminAuthorizationInfo>(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}   

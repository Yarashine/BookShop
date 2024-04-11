using Microsoft.EntityFrameworkCore;
using Models.Entities;
using System.Reflection;

namespace Repositories;

public partial class ShopContext : DbContext
{
    public ShopContext(DbContextOptions<ShopContext>  options) : base(options) { }
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
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<StateType>();
        modelBuilder.HasPostgresEnum<BlockedType>();

        modelBuilder.Ignore<Entity>();
        modelBuilder.Ignore<StringEntity>();
        modelBuilder.Ignore<AuthorizationInfo>();
        modelBuilder.Ignore<IfBlockedStatus>();


        modelBuilder.Entity<User>()
            .HasMany(a => a.PurchasedBooks)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId);
        modelBuilder.Entity<User>()
            .HasMany(u => u.Favorites)
            .WithMany(b => b.AddedInFavorites);
        modelBuilder.Entity<User>()
            .HasMany(u => u.Library)
            .WithMany(b => b.AddedInLibrary);
        modelBuilder.Entity<User>()
            .HasMany(u => u.BooksToSell)
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
            .HasOne(u => u.Status)
            .WithOne(s => s.User)
        .HasForeignKey<UserStatus>(s => s.UserId);
        modelBuilder.Entity<User>()
            .HasOne(u => u.UserImage)
            .WithOne(i => i.User)
            .HasForeignKey<UserImage>(i => i.UserId);

        modelBuilder.Entity<User>()
            .HasOne(u => u.AuthorizationInfo)
            .WithOne(i => i.User)
            .HasForeignKey<UserAuthorizationInfo>(a => a.UserId);

        modelBuilder.Entity<UserAuthorizationInfo>().HasKey(e => e.Email);
        modelBuilder.Entity<AdminAuthorizationInfo>().HasKey(e => e.Email);

        modelBuilder.Entity<Tag>().HasKey(s => s.Name);
        modelBuilder.Entity<Genre>().HasKey(s => s.Name);
        

        modelBuilder.Entity<Book>()
            .HasMany(b => b.Comments)
            .WithOne(c => c.Book)
            .HasForeignKey(c => c.BookId);
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Tags)
            .WithMany(t => t.Books);
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Genres)
            .WithMany(g => g.Books);
        modelBuilder.Entity<Book>()
            .HasOne(b => b.EBook)
            .WithOne(e => e.Book)
            .HasForeignKey<EBook>(e => e.BookId);

        modelBuilder.Entity<Book>()
            .HasOne(b => b.Status)
            .WithOne(s => s.Book)
        .HasForeignKey<BookStatus>(s=>s.BookId);
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Cover)
            .WithOne(i => i.Book)
            .HasForeignKey<BookImage>(i=>i.BookId);


        modelBuilder.Entity<Comment>()
            .HasMany(c => c.Reactions)
            .WithOne(r => r.Comment)
            .HasForeignKey(r => r.CommentId);
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Status)
            .WithOne(s => s.Comment)
            .HasForeignKey<CommentStatus>(s => s.CommentId);

        
        modelBuilder.Entity<Administrator>()
            .HasOne(a => a.AuthorizationInfo)
            .WithOne(i => i.User)
            .HasForeignKey<AdminAuthorizationInfo>(a => a.UserId);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}   

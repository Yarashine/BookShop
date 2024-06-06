namespace Models.Entities;

public class BookChangeLog : Entity
{
    public Guid BookId { get; set; }
    public string AuthorName { get; set; } = null!;
    public Book Book { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime DateCreated { get; set; }
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;
    public Guid EBookId { get; set; }
    public EBook EBook { get; set; } = null!; 
}

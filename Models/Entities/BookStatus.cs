
namespace Models.Entities;

public class BookStatus : IfBlockedStatus
{
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
}

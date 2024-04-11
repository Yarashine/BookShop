
namespace Models.Entities;

public class Genre : StringEntity
{
    override public List<Book> Books { get; set; } = new();
}

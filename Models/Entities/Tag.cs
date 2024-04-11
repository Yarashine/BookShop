
namespace Models.Entities;

public class Tag : StringEntity
{
    override public List<Book> Books { get; set; } = new();
}

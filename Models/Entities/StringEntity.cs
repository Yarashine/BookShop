
namespace Models.Entities;
 public class StringEntity
{
    public string Name { get; set; } = null!;
    virtual public List<Book> Books { get; set; } = new();
}


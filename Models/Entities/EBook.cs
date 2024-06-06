namespace Models.Entities;

public class EBook : Entity
{
    public byte[] Bytes { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public Guid BookId { get; set; }
    public Book Book { get; set; } = null!;
    public Guid ChangeLogId { get; set; }
    public BookChangeLog ChangeLog { get; set; } = null!;
}

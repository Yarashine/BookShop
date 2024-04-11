namespace Models.Entities;

public class EBook : Entity
{
    public byte[] Bytes { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public string FileName { get; set; } = null!;
    public Guid BookId { get; set; }
    public virtual Book Book { get; set; } = null!;
}

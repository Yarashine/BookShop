namespace Models.Entities;

public class UserImage : Entity
{
    public byte[] Bytes { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public string FileName { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}

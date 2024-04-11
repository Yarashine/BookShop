namespace Models.Entities;

public class Reaction : Entity
{
    public bool IsLike { get; set; }
    public Guid CommentId { get; set; }
    public Comment Comment { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}

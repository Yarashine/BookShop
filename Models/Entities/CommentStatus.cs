
namespace Models.Entities;

public class CommentStatus : IfBlockedStatus
{
    public Guid CommentId { get; set; }
    public Comment Comment { get; set; } = null!;
}

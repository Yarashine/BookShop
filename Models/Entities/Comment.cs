namespace Models.Entities;

public class Comment : Entity
{
    public Guid AuthorId { get; set; } 
    public virtual User Author { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public Guid BookId { get; set; } 
    public virtual Book Book { get; set; } = null!;
    public StateType State { get; set; } = StateType.IsExisted;
    public CommentStatus? Status { get; set; } = null;
    public DateTime CreatedDate { get; set; }
    public string? Description { get; set; } = null;
    public int Likes { get; set; } = 0;
    public int Dislikes { get; set; } = 0;
    public List<Reaction> Reactions { get; set; } = [];
}

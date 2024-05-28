namespace Models.Entities;

public class User : Entity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null;
    public UserImage? UserImage { get; set; } = null;
    public StateType State { get; set; } = StateType.IsExisted;
    public List<UserStatus> Status { get; set; } = []; 
    public List<Book> Library { get; set; } = [];
    public List<Book> Favorites { get; set; } = [];
    public List<Book> BooksToSell { get; set; } = [];
    public List<Book> PurchasedBooks { get; set; } = [];
    public List<Comment> Comments { get; set; } = [];
    public List<Reaction> Reactions { get; set; } = [];
    public int CommentaryViolations { get; set; } = 0;
    public UserAuthorizationInfo AuthorizationInfo { get; set; } = null!;
}

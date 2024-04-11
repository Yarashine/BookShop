namespace Models.Entities;

public class User : Entity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null;
    public UserImage? UserImage { get; set; } = null;
    public decimal Score { get; set; } = 0;
    public StateType State { get; set; } = StateType.IsExisted;
    public UserStatus? Status { get; set; } = null; 
    public List<Book> Library { get; set; } = new();
    public List<Book> Favorites { get; set; } = new();
    public List<Book> BooksToSell { get; set; } = new();
    public List<Book> PurchasedBooks { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public List<Reaction> Reactions { get; set; } = new();
    public UserAuthorizationInfo AuthorizationInfo { get; set; } = null!;
}

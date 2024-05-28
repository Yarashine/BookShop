
namespace Models.Entities;

public class UnbanRequest : Entity
{
    public DateTime TimeOfCreation { get; set; }
    public string? Description { get; set; } = null;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid StatusId { get; set; }
    public UserStatus Status { get; set; } = null !;
}

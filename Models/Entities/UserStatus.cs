
namespace Models.Entities;

public class UserStatus : IfBlockedStatus
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}

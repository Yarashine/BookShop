
namespace Models.Entities;

public class UserAuthorizationInfo:AuthorizationInfo
{
    public override Guid UserId { get; set; }
    public User User { get; set; } = null!;
}

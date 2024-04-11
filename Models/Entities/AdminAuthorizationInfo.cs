
namespace Models.Entities;

public class AdminAuthorizationInfo : AuthorizationInfo
{
    public override Guid UserId { get; set; }
    public Administrator User { get; set; } = null!;
}

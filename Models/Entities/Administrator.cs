namespace Models.Entities;

public class Administrator : Entity
{
    public string Name { get; set; } = null!;

    public AdminAuthorizationInfo AuthorizationInfo { get; set; } = null!;

}
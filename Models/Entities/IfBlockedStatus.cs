namespace Models.Entities;

public class IfBlockedStatus : Entity
{
    public StateType StateType { get; set; } = StateType.IsExisted;
    public DateTime DateTimeOfBan { get; set; }
    public string Description { get; set; } = null!;
    public Guid AdministratorId { get; set; }
    public Administrator Administrator { get; set; } = null!;
    public string NameOfAdmin { get; set; } = null!;

}

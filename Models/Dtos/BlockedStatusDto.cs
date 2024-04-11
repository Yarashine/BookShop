using Models.Entities;
namespace Models.Dtos;

public record BlockedStatusDto(Guid AdministratorId, string Description, Guid BlockedEntityId);



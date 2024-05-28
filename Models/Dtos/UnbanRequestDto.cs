using Models.Entities;

namespace Models.Dtos;

public record UnbanRequestDto(DateTime TimeOfCreation, 
    string? Description, Guid UserId);


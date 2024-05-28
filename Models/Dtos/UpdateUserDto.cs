using Models.Entities;
namespace Models.Dtos;

public record UpdateUserDto(string Name, MediaCreationDto? ImageDto, string? Description);

using Models.Entities;
namespace Models.Dtos;

public record UserDto(string Name, MediaCreationDto? ImageDto, string? Description);

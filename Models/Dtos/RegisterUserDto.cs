using Models.Entities;
namespace Models.Dtos;

public record RegisterUserDto(string Name, MediaCreationDto? ImageDto, string? Description, string Email, string Password);

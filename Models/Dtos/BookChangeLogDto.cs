using Models.Entities;

namespace Models.Dtos;

public record BookChangeLogDto(string? Description, MediaCreationDto EBookDto);

using Models.Entities;

namespace Models.Dtos;

public record SummaryBookDto(string Title, string? Description, List<string>? Tags, 
    List<string>? Genres, MediaCreationDto? ImageDto, Guid Id);

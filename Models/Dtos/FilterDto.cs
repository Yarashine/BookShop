using Models.Entities;

namespace Models.Dtos;

public record FilterDto(string? Title, int? Sort, List<string>? Tags, List<string>? Genres);

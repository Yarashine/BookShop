using Models.Entities;

namespace Models.Dtos;

public record BookDto(string Title, string? Description, Guid AuthorId, /*List<string>? tags,List<string>? genres,*/ MediaCreationDto? ImageDto, MediaCreationDto EBookDto,
                      string? Series, decimal? Price);

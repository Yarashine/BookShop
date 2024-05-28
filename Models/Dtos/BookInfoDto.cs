using Models.Entities;

namespace Models.Dtos;

public record BookInfoDto(string Title, string? Description, 
    List<string>? Tags, List<string>? Genres,
    MediaCreationDto? ImageDto, List<CommentDto> Comments,
    DateTime DateOfPublication, int Likes, string AuthorName,
                      string? Series, int? Price);


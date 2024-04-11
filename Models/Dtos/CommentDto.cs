namespace Models.Dtos;

public record CommentDto(Guid AuthorId, Guid BookId,string? Description);

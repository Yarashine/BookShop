
namespace Models.Dtos;

public record ReactionDto(bool IsLike, Guid UserId, Guid CommentId);


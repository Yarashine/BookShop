
namespace Models.Dtos;

public record ReactionDto(bool IsLike, bool IsDelete,Guid UserId, Guid CommentId);


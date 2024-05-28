using Models.Dtos;
using Models.Entities;

namespace Servises.Interfaces;

public interface ICommentService
{
    Task AddCommentAsync(CommentDto comment);
    Task DeleteCommentAsync(Guid userId, Guid commentId);
    Task UpdateReactionAsync(ReactionDto reaction);
}

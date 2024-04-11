using Models.Dtos;
using Models.Entities;

namespace Servises.Interfaces;

public interface ICommentService
{
    Task<bool> AddCommentAsync(CommentDto comment);
    //Task<bool> DeleteCommentAsync(Guid commentId);
    Task<bool> AddReactionToCommentAsync(ReactionDto reaction);
}

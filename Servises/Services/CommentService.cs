using Mapster;
using Models.Abstractions;
using Models.Dtos;
using Models.Entities;
using Models.Exceptions;
using Servises.Interfaces;
using System.ComponentModel.Design;

namespace Servises.Services;

public class CommentService(IUnitOfWork _unitOfWork) : BaseService(_unitOfWork), ICommentService
{
    public async Task AddCommentAsync(CommentDto comment)
    {
        User user = await unitOfWork.UserRepository.GetByIdAsync(comment.AuthorId)
            ?? throw new NotFoundException(nameof(User));
        Book book = await unitOfWork.BookRepository.GetByIdAsync(comment.BookId)
            ?? throw new NotFoundException(nameof(Book));
        Comment comment1 = comment.Adapt<Comment>();
        comment1.State = StateType.IsExisted;
        comment1.CreatedDate = DateTime.Now.ToUniversalTime();
        comment1.AuthorName=user.Name;
        book.Comments.Add(comment1);
        user.Comments.Add(comment1);
        await unitOfWork.SaveAllAsync();
    }

    public async Task UpdateReactionAsync(ReactionDto reaction)
    {
        Comment comment = await unitOfWork.CommentRepository.GetByIdAsync(reaction.CommentId)
            ?? throw new NotFoundException(nameof(Comment));
        User user = await unitOfWork.UserRepository.GetByIdWithReactionsAsync(reaction.UserId)
            ?? throw new NotFoundException(nameof(User));
        if (comment.AuthorId == reaction.UserId)
            throw new BadRequestException("Author of the reaction can't update a reaction");
        Reaction reaction1 = reaction.Adapt<Reaction>();
        if (reaction.IsDelete)
        {
            if (!user.Reactions.Any(r => r.UserId == reaction.UserId))
                throw new BadRequestException("The comment contains no reaction");
            var reaction2 = user.Reactions.Find(r => r.CommentId == reaction.CommentId)
                    ?? throw new BadRequestException("Update reaction error");
            user.Reactions.Remove(reaction2);
            if (reaction2.IsLike)
                comment.Likes--;
            else
                comment.Dislikes--;

        }
        else
        {
            if (!user.Reactions.Any(r => r.UserId == reaction.UserId))
            {
                user.Reactions.Add(reaction1);
                comment.Reactions.Add(reaction1);
                if (reaction.IsLike)
                    comment.Likes++;
                else
                    comment.Dislikes++;
            }
            else
            {
                var reaction2 = user.Reactions.Find(r => r.CommentId == reaction.CommentId)
                    ?? throw new BadRequestException("Update reaction error");
                if (reaction.IsLike && !reaction2.IsLike)
                {
                    comment.Likes++;
                    comment.Dislikes--;
                }
                else if (!reaction.IsLike && reaction2.IsLike)
                {
                    comment.Likes--;
                    comment.Dislikes++;
                }
            }
        }
        await unitOfWork.SaveAllAsync();
    }
    public async Task DeleteCommentAsync(Guid userId, Guid commentId)
    {
        Comment comment = await unitOfWork.CommentRepository.GetByIdAsync(commentId)
            ?? throw new NotFoundException(nameof(Comment));
        User user = await unitOfWork.UserRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        if (comment.AuthorId != userId)
            throw new BadRequestException("Only the author of the comment can delete a comment");
        await unitOfWork.CommentRepository.DeleteAsync(commentId);
        await unitOfWork.SaveAllAsync();
    }

}

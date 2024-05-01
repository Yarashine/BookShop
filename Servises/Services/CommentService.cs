using Mapster;
using Models.Abstractions;
using Models.Dtos;
using Models.Entities;
using Models.Exceptions;
using Servises.Interfaces;

namespace Servises.Services;

public class CommentService : BaseService, ICommentService
{
    public CommentService(IUnitOfWork _unitOfWork) : base(_unitOfWork) { }
    public async Task<bool> AddCommentAsync(CommentDto comment)
    {

        Comment comment1 = comment.Adapt<Comment>();
        User? user = await unitOfWork.UserRepository.GetByIdAsync(comment1.AuthorId);
        Book? book = await unitOfWork.BookRepository.GetByIdAsync(comment1.BookId);
        if (user is null)
            throw new NotFoundException(nameof(User));
        if (book is null)
            throw new NotFoundException(nameof(Book));
        comment1.State = StateType.IsExisted;
        comment1.CreatedDate = DateTime.Now.ToUniversalTime();
        comment1.AuthorName=user.Name;
        book.Comments.Add(comment1);
        user.Comments.Add(comment1);
        comment1.Author = user;
        comment1.Book = book;
        await unitOfWork.SaveAllAsync();
        return true;


    }

    public Task<bool> DeleteCommentAsync(Guid commentId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddReactionToCommentAsync(ReactionDto reaction)
    {
        Comment? comment = await unitOfWork.CommentRepository.GetByIdAsync(reaction.CommentId);
        User? user = await unitOfWork.UserRepository.GetByIdAsync(reaction.UserId);
        if (user is null)
            throw new NotFoundException(nameof(User));
        if (comment is null)
            throw new NotFoundException(nameof(Comment));
        Reaction reaction1 = reaction.Adapt<Reaction>();
        user.Reactions.Add(reaction1);
        comment.Reactions.Add(reaction1);
        if (reaction.IsLike)
            comment.Likes++;
        else
            comment.Dislikes++;
        reaction1.Comment = comment;
        reaction1.User = user;
        await unitOfWork.SaveAllAsync();
        return true;
    }
}

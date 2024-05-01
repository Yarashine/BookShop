using Mapster;
using Models.Abstractions;
using Models.Dtos;
using Models.Exceptions;
using Models.Entities;
using Servises.Interfaces;

namespace Servises.Services;

public class AdministratorService(IUnitOfWork _unitOfWork) : BaseService(_unitOfWork), IAdministratorService
{
    public async Task<bool> BlockBookAsync(BlockedStatusDto status)
    {
        Book? book = await unitOfWork.BookRepository.GetByIdAsync(status.BlockedEntityId);
        Administrator? administrator = await unitOfWork.AdministratorRepository.GetByIdAsync(status.AdministratorId);
        if (book is null)
            throw new NotFoundException(nameof(Book));
        if (administrator is null)
            throw new NotFoundException(nameof(Administrator));
        book.Status = status.Adapt<BookStatus>();
        book.Status.CountOfViolations++;
        book.Status.StateType=StateType.IsBlocked;
        book.Status.NameOfAdmin = administrator.Name;
        book.Status.Administrator = administrator;
        book.State = StateType.IsBlocked;
        await unitOfWork.SaveAllAsync();
        return true;
    }

    public async Task<bool> BlockCommentAsync(BlockedStatusDto status)
    {
        Comment? comment = await unitOfWork.CommentRepository.GetByIdAsync(status.BlockedEntityId);
        Administrator? administrator = await unitOfWork.AdministratorRepository.GetByIdAsync(status.AdministratorId);
        if (comment is null)
            throw new NotFoundException(nameof(Comment));
        if (administrator is null)
            throw new NotFoundException(nameof(Administrator));
        comment.Status = status.Adapt<CommentStatus>();
        comment.Status.CountOfViolations++;
        comment.Status.StateType = StateType.IsBlocked;
        comment.Status.NameOfAdmin = administrator.Name;
        comment.Status.Administrator = administrator;
        comment.State = StateType.IsBlocked;
        await unitOfWork.SaveAllAsync();
        return true;
    }

    public async Task<bool> BlockUserAsync(BlockedStatusDto status)
    {
        User? user = await unitOfWork.UserRepository.GetByIdWithStatusAsync(status.BlockedEntityId);
        Administrator? administrator = await unitOfWork.AdministratorRepository.GetByIdAsync(status.AdministratorId);
        if (user is null)
            throw new NotFoundException(nameof(User));
        if (administrator is null)
            throw new NotFoundException(nameof(Administrator));
        user.Status ??= status.Adapt<UserStatus>();
        user.Status.AdministratorId = status.AdministratorId;
        user.Status.Description = status.Description;
        user.Status.CountOfViolations++;
        user.Status.StateType = StateType.IsBlocked;
        user.Status.NameOfAdmin = administrator.Name;
        user.Status.Administrator = administrator;
        user.State = StateType.IsBlocked;
        await unitOfWork.SaveAllAsync();
        return true;
    }

    public async Task<bool> UnBlockBookAsync(Guid id)
    {
        Book? book = await unitOfWork.BookRepository.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Book));
        book.State = StateType.IsExisted;
        await unitOfWork.SaveAllAsync();
        return true;
    }

    public async Task<bool> UnBlockCommentAsync(Guid id)
    {
        Comment? comment = await unitOfWork.CommentRepository.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Comment));
        comment.State = StateType.IsExisted;
        await unitOfWork.SaveAllAsync();
        return true;
    }

    public async Task<bool> UnBlockUserAsync(Guid id)
    {
        User? user = await unitOfWork.UserRepository.GetByIdWithStatusAsync(id) ?? throw new NotFoundException(nameof(User));
        user.State = StateType.IsExisted;
        if(user.Status is not null)
        user.Status.StateType = StateType.IsExisted;
        await unitOfWork.SaveAllAsync();
        return true;
    }
}

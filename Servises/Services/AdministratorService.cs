using Mapster;
using Models.Abstractions;
using Models.Dtos;
using Models.Exceptions;
using Models.Entities;
using Servises.Interfaces;

namespace Servises.Services;

public class AdministratorService(IUnitOfWork _unitOfWork) : BaseService(_unitOfWork), IAdministratorService
{
    public async Task<List<UnbanRequestDto>> AllUnblockRequestsAsync()
    {
        var unbans = await unitOfWork.UnbanRequestRepository.ListAllAsync();
        List<UnbanRequestDto> unban = unbans.Adapt<List<UnbanRequestDto>>();
        return unban;
    }
    public async Task BlockBookAsync(BlockedStatusDto status)
    {
        Book book = await unitOfWork.BookRepository.GetByIdAsync(status.BlockedEntityId)
            ?? throw new NotFoundException(nameof(Book));
        Administrator administrator = await unitOfWork.AdministratorRepository.GetByIdAsync(status.AdministratorId)
            ?? throw new NotFoundException(nameof(Administrator));
        if (book.Status is null)
            throw new BadRequestException("Status is null");
        BookStatus Status = new()
        {
            AdministratorId = status.AdministratorId,
            Description = status.Description,
            StateType = StateType.IsBlocked,
            NameOfAdmin = administrator.Name,
            Administrator = administrator,
            DateTimeOfBan = DateTime.UtcNow,
        };
        book.State = StateType.IsBlocked;
        book.Status.Add(Status);
        await unitOfWork.SaveAllAsync();
    }

    public async Task BlockCommentAsync(BlockedStatusDto status)
    {
        Comment comment = await unitOfWork.CommentRepository.GetByIdAsync(status.BlockedEntityId)
            ?? throw new NotFoundException(nameof(Comment));
        Administrator administrator = await unitOfWork.AdministratorRepository.GetByIdAsync(status.AdministratorId)
            ?? throw new NotFoundException(nameof(Administrator));
        User user = await unitOfWork.UserRepository.GetByIdWithStatusAsync(comment.AuthorId)
            ?? throw new NotFoundException(nameof(User));
        user.CommentaryViolations++;
        comment.Status = status.Adapt<CommentStatus>();
        comment.Status.StateType = StateType.IsBlocked;
        comment.Status.NameOfAdmin = administrator.Name;
        comment.Status.Administrator = administrator;
        comment.Status.DateTimeOfBan = DateTime.UtcNow;
        comment.Status.AdministratorId = status.AdministratorId;
        comment.Status.Description = status.Description;
        comment.State = StateType.IsBlocked;
        if (user.CommentaryViolations >= 4)
        {
            BlockedStatusDto blockedStatusDto = new(status.AdministratorId, 
                "The user left 4 comments that violated community rules", user.Id);
            await BlockUserAsync(blockedStatusDto);
        }
        await unitOfWork.SaveAllAsync();
    }

    public async Task BlockUserAsync(BlockedStatusDto status)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithStatusAsync(status.BlockedEntityId)
            ?? throw new NotFoundException(nameof(User));
        Administrator administrator = await unitOfWork.AdministratorRepository.GetByIdAsync(status.AdministratorId)
            ?? throw new NotFoundException(nameof(Administrator));
        if(user.State == StateType.IsBlocked)
            throw new BadRequestException("User is already ban");
        if (user.Status is null)
            throw new BadRequestException("Status is null");
        UserStatus Status = new()
        {
            AdministratorId = status.AdministratorId,
            Description = status.Description,
            StateType = StateType.IsBlocked,
            NameOfAdmin = administrator.Name,
            Administrator = administrator,
            DateTimeOfBan = DateTime.UtcNow,

        };
        user.State = StateType.IsBlocked;
        user.Status.Add(Status);
        await unitOfWork.SaveAllAsync();
    }

    public async Task UnBlockBookAsync(Guid id)
    {
        Book book = await unitOfWork.BookRepository.GetByIdAsync(id) 
            ?? throw new NotFoundException(nameof(Book));
        book.State = StateType.IsExisted;
        await unitOfWork.SaveAllAsync();
    }

    public async Task UnBlockCommentAsync(Guid id)
    {
        Comment comment = await unitOfWork.CommentRepository.GetByIdAsync(id) 
            ?? throw new NotFoundException(nameof(Comment));
        comment.State = StateType.IsExisted;
        User user = await unitOfWork.UserRepository.GetByIdWithStatusAsync(comment.AuthorId)
            ?? throw new NotFoundException(nameof(User));
        user.CommentaryViolations--;
        await unitOfWork.SaveAllAsync();
    }

    public async Task UnBlockUserAsync(Guid id)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithStatusAsync(id) 
            ?? throw new NotFoundException(nameof(User));
        var list_request = await unitOfWork.UnbanRequestRepository.ListAsync(r => r.UserId == id);
        if (list_request != null)
        {
            foreach (var request in list_request)
            {
                await unitOfWork.UnbanRequestRepository.DeleteAsync(request.Id);
            }
        }
        user.CommentaryViolations = 0;
        user.State = StateType.IsExisted;
        await unitOfWork.SaveAllAsync();
    }
}

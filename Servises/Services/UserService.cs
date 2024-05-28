using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Abstractions;
using Servises.Interfaces;
using Models.Dtos;
using Mapster;
using Models.Exceptions;

namespace Servises.Services;

public class UserService(IUnitOfWork _unitOfWork) : BaseService(_unitOfWork), IUserService
{
    public async Task UnbanRequestAsync(Guid userId, string? description)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithStatusAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        if (user.State == StateType.IsExisted)
            throw new BadRequestException("User don't have block");
        if (user.Status == null)
            throw new BadRequestException("User don't have status");
        var request2 = await unitOfWork.UnbanRequestRepository.FirstOrDefaultAsync(r => r.UserId== userId);

        if (request2 != null)
            await unitOfWork.UnbanRequestRepository.DeleteAsync(request2.Id);
        var request = new UnbanRequest()
        {
            Description = description,
            User = user,
            UserId = userId,
            Status = user.Status.OrderBy(s => s.DateTimeOfBan).ToList().LastOrDefault(),
            TimeOfCreation = DateTime.Now.ToUniversalTime()
        };
        await unitOfWork.UnbanRequestRepository.AddAsync(request);
        await unitOfWork.SaveAllAsync();
    }

    public async Task UpdateUserAsync(Guid userId, UpdateUserDto user)
    {
        var user1 = await unitOfWork.UserRepository.GetByIdWithMediaAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        user1.Name = user.Name;
        user1.Description = user.Description;
        if (user.ImageDto is not null)
        {
            using MemoryStream memoryStream = new();
            await user.ImageDto.File.CopyToAsync(memoryStream);
            user1.UserImage = new UserImage()
            {
                UserId = user1.Id,
                FileName = user.ImageDto.File.FileName,
                FileType = user.ImageDto.File.ContentType,
                Bytes = memoryStream.ToArray()
            };
        }
        else
            user1.UserImage = null;
        await unitOfWork.UserRepository.UpdateAsync(user1);
        await unitOfWork.SaveAllAsync();
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        User user = await unitOfWork.UserRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(User));
        return user;
    }

    public async Task<List<SummaryBookDto>> GetFavoritesAsync(Guid id)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithFavoritesAsync(id)
            ?? throw new NotFoundException(nameof(User));
        return user.Favorites.Adapt<List<SummaryBookDto>>();
    }

    public async Task<List<SummaryBookDto>> GetLibraryAsync(Guid id)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithLibraryAsync(id)
            ?? throw new NotFoundException(nameof(User));
        return user.Library.Adapt<List<SummaryBookDto>>();
    }

    public async Task<List<SummaryBookDto>> GetBookToSellAsync(Guid id)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithBooksToSellAsync(id)
            ?? throw new NotFoundException(nameof(User));
        return user.BooksToSell.Adapt<List<SummaryBookDto>>();
    }

    public async Task<List<SummaryBookDto>> GetPurchasedBookAsync(Guid id)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithPurchasedBooksAsync(id)
            ?? throw new NotFoundException(nameof(User));
        return user.PurchasedBooks.Adapt<List<SummaryBookDto>>();
    }
    public async Task DeleteUserAsync(Guid id)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithFavoritesAsync(id)
            ?? throw new NotFoundException(nameof(User));
        foreach (var item in user.Favorites)
        {
            item.Likes--;
        }
        User user2 = await unitOfWork.UserRepository.GetByIdWithReactionsAsync(id)
             ?? throw new NotFoundException(nameof(User));
        foreach(var item in user2.Reactions)
        {
            Comment comment = await unitOfWork.CommentRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Comment));
            if(item.IsLike)
                comment.Likes--;
            else
                comment.Dislikes--;
        }
        await unitOfWork.UserRepository.DeleteAsync(id);
        await unitOfWork.SaveAllAsync();
    }
}

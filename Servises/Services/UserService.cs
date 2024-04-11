using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Models.Abstractions;
using Servises.Interfaces;
using Models.Dtos;
using Mapster;
using Repositories.Repositories;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace Servises.Services;

public class UserService : BaseService, IUserService
{
    public UserService(IUnitOfWork _unitOfWork) : base(_unitOfWork) { }

    public async Task<bool> AddUserAsync(RegisterUserDto user)
    {
        /*User? user2 = await unitOfWork.UserRepository.GetByIdAsync(user);
        if (user2 is null)
            return false;*/
        User user1 = user.Adapt<User>();
        if (user.ImageDto is not null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await user.ImageDto.File.CopyToAsync(memoryStream);
                user1.UserImage = new UserImage()
                {
                    UserId = user1.Id,
                    FileName = user.ImageDto.File.FileName,
                    FileType = user.ImageDto.File.ContentType,
                    Bytes = memoryStream.ToArray()
                };
            }
        }
        await unitOfWork.UserRepository.AddAsync(user1);
        await unitOfWork.SaveAllAsync();
        return true;
    }



    public async Task<bool> UpdateUserAsync(RegisterUserDto user)
    {
        //await unitOfWork.UserRepository.UpdateAsync(user.Adapt<User>());
        await unitOfWork.SaveAllAsync();
        return true;
    }

    public async Task<IReadOnlyList<User>> GetAllUsersAsync()
    {
        return await unitOfWork.UserRepository.ListAllAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        User? user = await unitOfWork.UserRepository.GetByIdAsync(id);
        return user;
    }

    public async Task<List<Book>?> GetFavoritesAsync(Guid id)
    {
        User? user = await unitOfWork.UserRepository.GetByIdWithFavoritesAsync(id);
        if (user is not null)
        {
            return user.Favorites;
        }
        return null;
    }

    public async Task<List<Book>?> GetLibraryAsync(Guid id)
    {
        User? user = await unitOfWork.UserRepository.GetByIdWithLibraryAsync(id);
        if (user is not null)
        return user.Library;
        return null;
    }

    public async Task<List<Book>?> GetBookToSellAsync(Guid id)
    {
        User? user = await unitOfWork.UserRepository.GetByIdWithBooksToSellAsync(id);
        if (user is not null)
        return user.BooksToSell;
        return null;
    }

    public async Task<List<Book>?> GetPurchasedBookAsync(Guid id)
    {
        User? user = await unitOfWork.UserRepository.GetByIdWithPurchasedBooksAsync(id);
        if (user is not null)
        return user.PurchasedBooks;
        return null;
    }

    public async Task<User?> GetByIdWithComments(Guid id)
    {
        User? user = await unitOfWork.UserRepository.GetByIdWithCommentsAsync(id);
        return user;
    }
}

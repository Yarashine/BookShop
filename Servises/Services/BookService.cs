using Mapster;
using Microsoft.AspNetCore.Http;
using Models.Abstractions;
using Models.Dtos;
using Models.Entities;
using Models.Exceptions;
using Servises.Interfaces;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Policy;
using Microsoft.AspNetCore.Mvc;
using Repositories.Repositories;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Servises.Services;

public class BookService(IUnitOfWork _unitOfWork, IPaymentService paymentService) : BaseService(_unitOfWork), IBookService
{
    public async Task DeleteBook(Guid userId, Guid bookId)
    {
        Book book = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        if (book.AuthorId != userId)
            throw new BadRequestException("User is not the author of this book");
        await unitOfWork.BookRepository.DeleteAsync(bookId);
        await unitOfWork.SaveAllAsync();
    }
    public async Task<IReadOnlyList<SummaryBookDto>> GetBooksAsync(FilterDto filter)
    {
        var books = await unitOfWork.BookRepository.ListAsync(b =>
        (string.IsNullOrEmpty(filter.Title) || EF.Functions.Like(b.Title, $"%{filter.Title}%")) &&
        (filter.Tags == null || filter.Tags.Count == 0 || filter.Tags.All(tag => b.Tags.Select(t => t.Name).Contains(tag))) &&
        (filter.Genres == null || filter.Genres.Count == 0 || filter.Genres.All(genre => b.Genres.Select(g => g.Name).Contains(genre)))
        );
        if (filter.Sort is null || filter.Sort == 0)
        {
            books = [.. books.OrderBy(b => b.Title)];
        }
        else if(filter.Sort == 1)
        {
            books = [.. books.OrderByDescending(b => b.Likes)];
        }
        var books1 = books.Adapt<List<SummaryBookDto>>();
        return books1;
    }
    public async Task AddLikeToBookAsync(Guid userId, Guid bookId)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithFavoritesAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        Book book = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        if (book.AuthorId == userId)
            throw new BadRequestException("The author cannot like his work");
        if (user.Favorites.Contains(book))
            throw new BadRequestException("Like this book already post");
        user.Favorites.Add(book);
        book.Likes++;
        await unitOfWork.SaveAllAsync();
    }

    public async Task<string> BuyBookAsync(Guid userId, Guid bookId)
    {
        var user = await unitOfWork.UserRepository.GetByIdWithAiAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        var book = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        if (user.PurchasedBooks.Any(x => x.Id == bookId))
            throw new BadRequestException("This book already purchase");
        if (book.AuthorId == userId)
            throw new BadRequestException("The author cannot buy his work");
        if (book.Price is null || book.Price == 0)
            throw new BadRequestException("Book is free");
        BuyBookDto bookDto = book.Adapt<BuyBookDto>();
        return await paymentService.Charge(user.Id,user.AuthorizationInfo.Email, bookDto);
    }
    public async Task WebHook(string json, HttpRequest request)
    {
        var stripeEvent = paymentService.EventFromJson(json, request);

        if (stripeEvent.Type == Events.CheckoutSessionCompleted)
        {
            var session = stripeEvent.Data.Object as Session 
                ?? throw new BadRequestException("Create session error. ");
            var bookId = session.Metadata["bookId"];
            var userId = session.Metadata["userId"];

            var user = await unitOfWork.UserRepository.GetByIdWithPurchasedBooksAsync(Guid.Parse(userId)) 
                ?? throw new NotFoundException(nameof(User));
            var book = await unitOfWork.BookRepository.GetByIdAsync(Guid.Parse(bookId)) 
                ?? throw new NotFoundException(nameof(Book));

            if (user.PurchasedBooks.Any(x => x.Id.ToString() == bookId))
                throw new BadRequestException("This book already purchase");
            if (book.AuthorId.ToString() == userId)
                throw new BadRequestException("The author cannot buy his work");
            if (book.Price is null || book.Price == 0)
                throw new BadRequestException("Book is free");

            user.PurchasedBooks.Add(book);
            await unitOfWork.SaveAllAsync();
        }
        else
        {
            Debug.WriteLine("Unhandled event type:" + stripeEvent.Type);
        }

    }

    public async Task AddBookAsync(Guid userId, BookDto book)
    {
        if (book.Price<0)
            throw new BadRequestException("The price must be positive");
        User user = await unitOfWork.UserRepository.GetByIdWithBooksToSellAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        Book book1 = new()
        {
            Series = book.Series,
            Title = book.Title,
            Description = book.Description,
            Price = book.Price,
            AuthorName = user.Name,
            AuthorId = userId
        };

        if (book.Tags is not null)
        {
            foreach (var tag in book.Tags)
            {
                Tag tag1 = await unitOfWork.TagRepository.GetByNameAsync(tag)
                   ?? throw new NotFoundException(nameof(Tag));
                book1.Tags.Add(tag1);
            }
        }
        if (book.Genres is not null)
        {
            foreach (var genre in book.Genres)
            {
                Genre genre1 = await unitOfWork.GenreRepository.GetByNameAsync(genre)
                   ?? throw new NotFoundException(nameof(Genre));
                book1.Genres.Add(genre1);
            }
        }
        if (book.ImageDto is not null)
        {
            using var memoryStream = new MemoryStream();
            await book.ImageDto.File.CopyToAsync(memoryStream);
            book1.Cover = new BookImage()
            {
                BookId = book1.Id,
                FileName = book.ImageDto.File.FileName,
                FileType = book.ImageDto.File.ContentType,
                Bytes = memoryStream.ToArray()
            };
        }
        using (var memoryStream = new MemoryStream())
        {
            await book.EBookDto.File.CopyToAsync(memoryStream);
            book1.EBook = new EBook()
            {
                BookId = book1.Id,
                FileName = book.EBookDto.File.FileName,
                FileType = book.EBookDto.File.ContentType,
                Bytes = memoryStream.ToArray(),
                Book = book1
            };
        }
        book1.DateOfPublication = DateTime.Now.ToUniversalTime();
        await unitOfWork.BookRepository.AddAsync(book1);
        user.BooksToSell.Add(book1);
        await unitOfWork.SaveAllAsync();
    }

    public async Task UpdateBookAsync(Guid userId, Guid bookId, UpdateBookDto book)
    {
        Book book1 = await unitOfWork.BookRepository.GetByIdWithAllAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));

        if (book1.AuthorId != userId)
            throw new BadRequestException("User is not the author of this book");
        
        
        if (book.EBookDto is not null)
        {
            await unitOfWork.BookRepository.DeleteAsync(bookId);
            BookDto dto = new(
                book.Title,
                book.Description,
                 book.Tags,
                 book.Genres,
                book.ImageDto,
                 book.EBookDto,
                book.Series,
                 book.Price);
            await unitOfWork.SaveAllAsync();
            await AddBookAsync(userId, dto);
        }
        else
        {
            if (book.Tags is not null)
            {
                foreach (var tag in book.Tags)
                {
                    Tag tag1 = await unitOfWork.TagRepository.GetByNameAsync(tag)
                       ?? throw new NotFoundException(nameof(Tag));
                    book1.Tags.Add(tag1);
                }
            }
            else
            {
                book1.Tags = [];
            }
            if (book.Genres is not null)
            {
                foreach (var genre in book.Genres)
                {
                    Genre genre1 = await unitOfWork.GenreRepository.GetByNameAsync(genre)
                       ?? throw new NotFoundException(nameof(Tag));
                    book1.Genres.Add(genre1);
                }
            }
            else
            {
                book1.Genres = [];
            }

            if (book.ImageDto is not null)
            {
                using var memoryStream = new MemoryStream();
                await book.ImageDto.File.CopyToAsync(memoryStream);
                book1.Cover = new BookImage()
                {
                    BookId = book1.Id,
                    FileName = book.ImageDto.File.FileName,
                    FileType = book.ImageDto.File.ContentType,
                    Bytes = memoryStream.ToArray()
                };
            }
            else
                book1.Cover = null;
            book1.Series = book.Series;
            book1.Title = book.Title;
            book1.Description = book.Description;
            book1.Price = book.Price;
            await unitOfWork.BookRepository.UpdateAsync(book1);
            await unitOfWork.SaveAllAsync();
        }
    }

    public async Task DeleteLikeFromBookAsync(Guid userId, Guid bookId)
    {
        User? user = await unitOfWork.UserRepository.GetByIdWithFavoritesAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        Book? book = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        if (book.AuthorId == userId)
            throw new BadRequestException("The author cannot delete like his work");
        if (!user.Favorites.Contains(book))
            throw new BadRequestException("The book is not in the Favorites");
        user.Favorites.Remove(book);
        book.Likes--;
        await unitOfWork.SaveAllAsync();
    }

    public async Task DeleteFromLibraryAsync(Guid userId, Guid bookId)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithLibraryAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        Book? book = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        if (book.AuthorId == userId)
            throw new BadRequestException("The author cannot add his work in Library");
        if (!user.Library.Contains(book))
            throw new BadRequestException("The book is not in the Library");
        user.Library.Remove(book);
        await unitOfWork.SaveAllAsync();
    }

    public async Task<BookInfoDto> GetByIdAsync(Guid id)
    {
        Book book = await unitOfWork.BookRepository.GetByIdWithAllAsync(id)
            ?? throw new NotFoundException(nameof(Book));
        /*var book1 = book.Adapt<BookInfoDto>();
        book1.ImageDto = book.Cover;*/
        return book.Adapt<BookInfoDto>();
    }

    public async Task<EBook> DownloadBookAsync(Guid userId, Guid id)
    {
        Book book = await unitOfWork.BookRepository.GetByIdWithBookAsync(id)
            ?? throw new NotFoundException(nameof(Book));
        User user = await unitOfWork.UserRepository.GetByIdWithPurchasedBooksAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        if (book.Price is null || book.Price == 0)
            return book.EBook;
        if (!user.PurchasedBooks.Contains(book))
            throw new BadRequestException("Buy a book to read it");
        return book.EBook;
    }

    public async Task AddToLibraryAsync(Guid userId, Guid bookId)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithLibraryAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        Book? book = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        if (book.AuthorId == userId)
            throw new BadRequestException("The author cannot add his work in Library");
        if (user.Library.Contains(book))
            throw new BadRequestException("This book already in the Library");
        user.Library.Add(book);
        await unitOfWork.SaveAllAsync();
    }
}

using Mapster;
using Models.Abstractions;
using Models.Dtos;
using Models.Entities;
using Models.Exceptions;
using Servises.Interfaces;

namespace Servises.Services;

public class BookService(IUnitOfWork _unitOfWork, IPaymentService paymentService) : BaseService(_unitOfWork), IBookService
{
    public async Task<bool> AddLikeToBookAsync(Guid userId, Guid bookId)
    {
        User? user = await unitOfWork.UserRepository.GetByIdWithFavoritesAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        Book? book = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book)); 
        user.Favorites.Add(book);
        book.Likes++;
        await unitOfWork.SaveAllAsync();
        return true;
    }

    public async Task<bool> BuyBookAsync(Guid userId, Guid bookId)
    {
        var user = await unitOfWork.UserRepository.GetByIdWithPurchasedBooksAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        var book = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        if (user.PurchasedBooks.Any(x => x.Id == bookId))
            throw new BadRequestException("This book already purchase");


        paymentService.Charge(user, book.Price ?? throw new BadRequestException("Book is free"));


        user.PurchasedBooks.Add(book);
        await unitOfWork.SaveAllAsync();
        return true;
    }

    public async Task<bool> AddBookAsync(BookDto book)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithBooksToSellAsync(book.AuthorId)
            ?? throw new NotFoundException(nameof(User));
        
        Book book1 = book.Adapt<Book>();
        book1.AuthorName = user.Name;
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
        book1.DateOfPublication= DateTime.Now.ToUniversalTime();

        await unitOfWork.BookRepository.AddAsync(book1);
        await unitOfWork.SaveAllAsync();
        return true;
    }

    public async Task<bool> UpdateBookAsync(Guid bookId, BookDto book)
    {
        User? user = await unitOfWork.UserRepository.GetByIdAsync(book.AuthorId)
            ?? throw new NotFoundException(nameof(User));
        Book? book1 = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        if (book1.AuthorId==book.AuthorId)
        {
            book1 = book.Adapt<Book>();
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
            book1.Title = book.Title;
            book1.Id = bookId;
            await unitOfWork.BookRepository.UpdateAsync(book1);
            await unitOfWork.SaveAllAsync();
            return true;
        }
        throw new BadRequestException("User is not the author of this book");
    }

    public async Task<bool> DeleteLikeFromBookAsync(Guid userId, Guid bookId)
    {
        User? user = await unitOfWork.UserRepository.GetByIdWithFavoritesAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        Book? book = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        if (book.AuthorId!=userId)
            throw new BadRequestException("User is not the author of this book");
        user.Favorites.Remove(book);
        book.Likes--;
        await unitOfWork.SaveAllAsync();
        return true;
    }

    public async Task<bool> DeleteFromLibraryAsync(Guid userId, Guid bookId)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithLibraryAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        Book? book = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        Book book2 = user.Library.FirstOrDefault(b => b.Id == bookId)
            ?? throw new NotFoundException(nameof(Book));
        user.Library.Remove(book);
        await unitOfWork.SaveAllAsync();
        return true;
    }

    public async Task<IReadOnlyList<Book>> GetAllAsync()
    {
        return await unitOfWork.BookRepository.ListAllAsync();
    }

    public async Task<Book?> GetByIdAsync(Guid id)
    {
        return await unitOfWork.BookRepository.GetByIdWithBookAndMediaAsync(id);
    }

    public async Task<EBook> DownloadBookAsync(Guid id)
    {
        Book book = await unitOfWork.BookRepository.GetByIdWithBookAsync(id)
            ?? throw new NotFoundException(nameof(Book));
        return book.EBook;
    }

    public async Task<bool> AddToLibraryAsync(Guid userId, Guid bookId)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithLibraryAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        Book? book = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        Book? book2 = user.Library.FirstOrDefault(b => b.Id == bookId);
        if (book2 is not null)
            throw new BadRequestException("This book is already in the library");
        user.Library.Add(book);
        await unitOfWork.SaveAllAsync();
        return true;
    }
}

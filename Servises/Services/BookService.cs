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
using Microsoft.EntityFrameworkCore;

namespace Servises.Services;

public class BookService(IUnitOfWork _unitOfWork, IPaymentService paymentService) : BaseService(_unitOfWork), IBookService
{
    public async Task AddCoAuthorAsync(Guid bookId, Guid coAuthorId, Guid userId)
    {
        var book = await _unitOfWork.BookRepository.GetByIdWithAllAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        if (book.AuthorId != userId)
            throw new UnauthorizedAccessException("Только основной автор может добавлять соавторов.");
        var coAuthor = await _unitOfWork.UserRepository.GetByIdAsync(coAuthorId)
            ?? throw new NotFoundException(nameof(User));
        book.CoAuthors.Add(coAuthor);
        await _unitOfWork.SaveAllAsync();
    }

    public async Task RemoveCoAuthorAsync(Guid bookId, Guid coAuthorId, Guid userId)
    {
        var book = await _unitOfWork.BookRepository.GetByIdWithAllAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        if (book.AuthorId != userId)
            throw new UnauthorizedAccessException("Только основной автор может добавлять соавторов.");
        var coAuthor = await _unitOfWork.UserRepository.GetByIdAsync(coAuthorId)
            ?? throw new NotFoundException(nameof(User));
        book.CoAuthors.Remove(coAuthor);
        await _unitOfWork.SaveAllAsync();
    }
    public async Task AddBookChangeLogAsync(Guid bookId, BookChangeLogDto changeLogDto, Guid userId)
    {

        var book = await _unitOfWork.BookRepository.GetByIdWithAllAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException(nameof(User));

        if (book.AuthorId != userId && !book.CoAuthors.Any(ca => ca.Id == userId))
            throw new BadRequestException("Only the author or co-authors can add a change log.");

        using (var memoryStream = new MemoryStream())
        {
            const long maxFileSize = 30 * 1024 * 1024;
            if (changeLogDto.EBookDto.File.Length > maxFileSize)
            {
                throw new InvalidOperationException("Размер файла превышает допустимый лимит.");
            }

            var allowedFileTypes = new List<string>
                {
                    "application/pdf", // PDF
                    "application/epub+zip", // EPUB
                    "application/x-fictionbook+xml", // FB2
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // DOCX
                    "application/msword", // DOC
                    "text/plain", // TXT
                    "application/x-mobipocket-ebook", // MOBI
                    "application/vnd.amazon.ebook", // AZW
                    "application/x-cbz", // CBZ
                    "application/x-cbr" // CBR
                };
            if (!allowedFileTypes.Contains(changeLogDto.EBookDto.File.ContentType))
            {
                throw new InvalidOperationException("Недопустимый тип файла.");
            }

            await changeLogDto.EBookDto.File.CopyToAsync(memoryStream);

            var eBook = new EBook()
            {
                Book = book,
                BookId = book.Id,
                FileName = changeLogDto.EBookDto.File.FileName,
                FileType = changeLogDto.EBookDto.File.ContentType,
                Bytes = memoryStream.ToArray(),
                Id = Guid.NewGuid(),
            };

            book.ReleaseEBook = eBook.Id;
            var changeLog = new BookChangeLog
            {
                Book = book,
                BookId = book.Id,
                Description = changeLogDto.Description,
                EBook = eBook,
                DateCreated = DateTime.UtcNow,
                CreatedById = userId,
                AuthorName = user.Name,
                Id = Guid.NewGuid(),
            };

            eBook.ChangeLog = changeLog;
            eBook.ChangeLogId = changeLog.Id;

            await _unitOfWork.BookChangeLogRepository.AddAsync(changeLog);
            await _unitOfWork.EBookRepository.AddAsync(eBook);

            book.ChangeLogs.Add(changeLog);
            book.EBooks.Add(eBook);
            await _unitOfWork.SaveAllAsync();
        }

    }

    public async Task<IReadOnlyList<BookChangeLogInfoDto>> GetBookChangeLogsAsync(Guid userId, Guid bookId)
    {
        var book = await _unitOfWork.BookRepository.GetByIdWithAllAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        var books = book.ChangeLogs.Adapt<List<BookChangeLogInfoDto>>();
        return books.OrderBy(cl => cl.DateCreated).ToList();
    }

    public async Task AddEBookFromChangeLogAsync(Guid bookId, Guid changeLogId, Guid userId)
    {
        var book = await _unitOfWork.BookRepository.GetByIdWithAllAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));

        var changeLog = book.ChangeLogs.FirstOrDefault(cl => cl.Id == changeLogId)
            ?? throw new NotFoundException(nameof(BookChangeLog));

        if (book.AuthorId != userId) 
            throw new UnauthorizedAccessException("Only the author can add EBook from ChangeLog");

        book.ReleaseEBook = changeLog.EBookId;
        await _unitOfWork.BookRepository.UpdateAsync(book);
        await _unitOfWork.SaveAllAsync();
    }
    public async Task DeleteBook(Guid userId, Guid bookId)
    {
        Book book = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        if (book.AuthorId != userId)
            throw new BadRequestException("User is not the author of this book");
        await unitOfWork.BookRepository.DeleteAsync(bookId);
        await unitOfWork.SaveAllAsync();
    }
    public async Task<IReadOnlyList<SummaryBookDto>> GetBooksAsync(Guid? userId, FilterDto filter)
    {
        var books = await unitOfWork.BookRepository.ListAsync(b =>
        (string.IsNullOrEmpty(filter.Title) || EF.Functions.Like(b.Title, $"%{filter.Title}%")) &&
        (filter.Tags == null || filter.Tags.Count == 0 || filter.Tags.All(tag => b.Tags.Select(t => t.Name).Contains(tag))) &&
        (filter.Genres == null || filter.Genres.Count == 0 || filter.Genres.All(genre => b.Genres.Select(g => g.Name).Contains(genre)))
        );
        if (filter.Sort is null || filter.Sort == 0)
        {
            if(userId is not  null)
            {
                var user1 = await unitOfWork.UserRepository.GetByIdWithPurchasedBooksAsync(userId.Value);
                var user2 = await unitOfWork.UserRepository.GetByIdWithFavoritesAsync(userId.Value);
                if (user1 != null && user2 != null)
                {
                    List<Book> userBooks = user1.PurchasedBooks.Concat(user2.Favorites).ToList();
                    var genresCount = userBooks
                        .SelectMany(b => b.Genres)
                        .GroupBy(g => g.Name)
                        .ToDictionary(g => g.Key, g => g.Count());

                    var tagsCount = userBooks
                        .SelectMany(b => b.Tags)
                        .GroupBy(t => t.Name)
                        .ToDictionary(t => t.Key, t => t.Count());

                    var sortedBooks = books
                        .Select(book => new
                        {
                            Book = book,
                            Weight = book.Genres.Sum(g => genresCount.TryGetValue(g.Name, out int value) ? value : 0) +
                                     book.Tags.Sum(t => tagsCount.TryGetValue(t.Name, out int value) ? value : 0)
                        })
                        .OrderByDescending(b => b.Weight)
                        .Select(b => b.Book)
                        .ToList();
                }
            }
            else
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
            if (book.ImageDto.File == null || book.ImageDto.File.Length == 0)
            {
                throw new InvalidOperationException("The file is not loaded or is empty.");
            }

            var allowedImageTypes = new[] { "image/jpeg", "image/png"};
            if (!allowedImageTypes.Contains(book.ImageDto.File.ContentType))
            {
                throw new InvalidOperationException("The file must be an image (jpeg, png).");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png"};
            var fileExtension = Path.GetExtension(book.ImageDto.File.FileName)?.ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("The file must have one of the following extensions: .jpg, .jpeg, .png.");
            }

            const long maxFileSizeInBytes = 5 * 1024 * 1024; 
            if (book.ImageDto.File.Length > maxFileSizeInBytes)
            {
                throw new InvalidOperationException("The file size must not exceed 5MB.");
            }

            using var memoryStream = new MemoryStream();
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
            const long maxFileSize = 30 * 1024 * 1024; 
            if (book.EBookDto.File.Length > maxFileSize)
            {
                throw new InvalidOperationException("Размер файла превышает допустимый лимит.");
            }

            var allowedFileTypes = new List<string>
            {
                "application/pdf", // PDF
                "application/epub+zip", // EPUB
                "application/x-fictionbook+xml", // FB2
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // DOCX
                "application/msword", // DOC
                "text/plain", // TXT
                "application/x-mobipocket-ebook", // MOBI
                "application/vnd.amazon.ebook", // AZW
                "application/x-cbz", // CBZ
                "application/x-cbr" // CBR
            };
            if (!allowedFileTypes.Contains(book.EBookDto.File.ContentType))
            {
                throw new InvalidOperationException("Недопустимый тип файла.");
            }
            await book.EBookDto.File.CopyToAsync(memoryStream);
            var eBook = new EBook()
            {
                Book = book1,
                FileName = book.EBookDto.File.FileName,
                FileType = book.EBookDto.File.ContentType,
                Bytes = memoryStream.ToArray(),
                Id = Guid.NewGuid(),
            };
            book1.ReleaseEBook = eBook.Id;
            var changeLog = new BookChangeLog
            {
                Book = book1,
                Description = "Первый экземпляр книги",
                EBook = eBook,
                DateCreated = DateTime.UtcNow,
                CreatedById = userId,
                AuthorName = user.Name,
                Id=Guid.NewGuid(),
            };
            eBook.ChangeLog = changeLog;
            eBook.ChangeLogId = changeLog.Id;
            book1.ChangeLogs.Add(changeLog);
            book1.EBooks.Add(eBook);
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
        {
            var ebook = await unitOfWork.EBookRepository.GetByIdAsync(book.ReleaseEBook)
                ?? throw new NotFoundException(nameof(EBook));
            return ebook;
        }
        if (!user.PurchasedBooks.Contains(book))
            throw new BadRequestException("Buy a book to read it");
        var ebook2 = await unitOfWork.EBookRepository.GetByIdAsync(book.ReleaseEBook)
                ?? throw new NotFoundException(nameof(EBook));
        return ebook2;
    }

    public async Task AddToLibraryAsync(Guid userId, Guid bookId)
    {
        User user = await unitOfWork.UserRepository.GetByIdWithLibraryAsync(userId)
            ?? throw new NotFoundException(nameof(User));
        Book book = await unitOfWork.BookRepository.GetByIdAsync(bookId)
            ?? throw new NotFoundException(nameof(Book));
        if (book.AuthorId == userId)
            throw new BadRequestException("The author cannot add his work in Library");
        if (user.Library.Contains(book))
            throw new BadRequestException("This book already in the Library");
        user.Library.Add(book);
        await unitOfWork.SaveAllAsync();
    }
}

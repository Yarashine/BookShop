using Models.Abstractions;
using Models.Entities;
using Servises.Interfaces;
using Models.Exceptions;

namespace Servises.Services;

public class GenreService : BaseService, IGenreService
{
    public GenreService(IUnitOfWork _unitOfWork) : base(_unitOfWork) { }

    public async Task AddGenreAsync(string name)
    {
        Genre? genre = await unitOfWork.GenreRepository.GetByNameAsync(name);
        if (genre is not null)
            throw new BadRequestException("This Genre is already exist");
        await unitOfWork.GenreRepository.AddAsync(new Genre() { Name = name });
        await unitOfWork.SaveAllAsync();
    }

    public async Task AddGenreToBookAsync(string name, Guid id)
    {
        Genre genre = await unitOfWork.GenreRepository.GetByNameWithBooksAsync(name)
            ?? throw new NotFoundException(nameof(Genre));
        Book book = await unitOfWork.BookRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Book));
        genre.Books.Add(book);
        await unitOfWork.SaveAllAsync();
    }

    public async Task DeleteGenreAsync(string name)
    {
        Genre genre = await unitOfWork.GenreRepository.GetByNameAsync(name)
            ?? throw new NotFoundException(nameof(Genre));
        await unitOfWork.GenreRepository.DeleteAsync(name);
        await unitOfWork.SaveAllAsync();
    }
    public async Task<IReadOnlyList<string>> AllGenresAsync()
    {
        IReadOnlyList<Genre> genres = await unitOfWork.GenreRepository.ListAllAsync();
        IReadOnlyList<string> strings = genres.Select(genres => genres.Name).ToList();
        return strings;
    }
}

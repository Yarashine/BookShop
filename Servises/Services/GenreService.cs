using Models.Abstractions;
using Models.Entities;
using Servises.Interfaces;

namespace Servises.Services;

public class GenreService : BaseService, IGenreService
{
    public GenreService(IUnitOfWork _unitOfWork) : base(_unitOfWork) { }

    public async Task<bool> AddGenreAsync(string name)
    {
        StringEntity? genre = await unitOfWork.GenreRepository.GetByNameAsync(name);
        if (genre is null)
        {
            await unitOfWork.GenreRepository.AddAsync(new Genre() { Name = name });
            await unitOfWork.SaveAllAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> AddGenreToBookAsync(string name, Guid id)
    {
        StringEntity? genre = await unitOfWork.GenreRepository.GetByNameWithBooksAsync(name);
        Book? book = await unitOfWork.BookRepository.GetByIdAsync(id);
        if (genre is not null && book is not null)
        {
            genre.Books.Add(book);
            //book.Genres.Add(genre);
            await unitOfWork.SaveAllAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteGenreAsync(string name)
    {
        StringEntity? genre = await unitOfWork.GenreRepository.GetByNameAsync(name);
        if (genre is null)
        {
            await unitOfWork.GenreRepository.DeleteAsync(name);
            await unitOfWork.SaveAllAsync();
            return true;
        }
        return false;
    }
    public async Task<IReadOnlyList<string>> AllGenresAsync()
    {
        IReadOnlyList<StringEntity> genres =await unitOfWork.GenreRepository.ListAllAsync();
        IReadOnlyList<string> strings = genres.Select(genres => genres.Name).ToList(); 
        return strings;
    }
}

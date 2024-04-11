
using Models.Entities;

namespace Servises.Interfaces;

public interface IGenreService
{
    Task<bool> AddGenreAsync(string name);
    Task<bool> AddGenreToBookAsync(string name, Guid id);
    Task<bool> DeleteGenreAsync(string name);
    Task<IReadOnlyList<string>> AllGenresAsync();
}

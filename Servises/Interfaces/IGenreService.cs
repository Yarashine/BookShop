
using Models.Entities;

namespace Servises.Interfaces;

public interface IGenreService
{
    Task AddGenreAsync(string name);
    Task AddGenreToBookAsync(string name, Guid id);
    Task DeleteGenreAsync(string name);
    Task<IReadOnlyList<string>> AllGenresAsync();
}

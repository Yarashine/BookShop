using Microsoft.AspNetCore.Mvc;
using Servises.Interfaces;

namespace Api.Controllers;

[ApiController]
[Route("/api")]
public class GenreController : Controller
{
    private readonly IGenreService genreService;
    public GenreController(IGenreService _genreService)
    {
        genreService = _genreService;
    }
    [HttpPost("genre/add/{genre}")]
    public async Task<IActionResult> AddGenre(string genre)
    {
        bool result = await genreService.AddGenreAsync(genre);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpPost("genre/{name}/book/{id}")]
    public async Task<IActionResult> AddGenreToBook(string name, Guid id)
    {
        bool result = await genreService.AddGenreToBookAsync(name, id);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpDelete("genre/{name}")]
    public async Task<IActionResult> DeleteGenre(string name)
    {
        bool result = await genreService.DeleteGenreAsync(name);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpGet("genres")]
    public async Task<IReadOnlyList<string>> GetAllAsync()
    {
        return await genreService.AllGenresAsync();
    }


}

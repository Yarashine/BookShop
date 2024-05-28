using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servises.Interfaces;

namespace Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("/api")]
public class GenreController(IGenreService genreService) : Controller
{

    [HttpPost("genre/add/{genre}")]
    public async Task<IActionResult> AddGenre(string genre)
    {
        await genreService.AddGenreAsync(genre);
        return Ok();
    }
    [HttpDelete("genre/{name}")]
    public async Task<IActionResult> DeleteGenre(string name)
    {
        await genreService.DeleteGenreAsync(name);
        return Ok();
    }
    [HttpGet("genres")]
    public async Task<IReadOnlyList<string>> GetAllAsync()
    {
        return await genreService.AllGenresAsync();
    }


}

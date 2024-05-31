using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servises.Interfaces;

namespace Api.Controllers;

[ApiController]
[Route("/api")]
public class GenreController(IGenreService genreService) : Controller
{
    [Authorize(Roles = "Admin")]
    [HttpPost("genre/add/{genre}")]
    public async Task<IActionResult> AddGenre(string genre)
    {
        await genreService.AddGenreAsync(genre);
        return Ok();
    }
    [Authorize(Roles = "Admin")]
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

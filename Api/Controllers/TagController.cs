/*using Microsoft.AspNetCore.Mvc;
using Servises.Interfaces;
using Servises.Services;

namespace Api.Controllers;

[ApiController]
[Route("/api")]
public class TagController : Controller
{
    private readonly ITagService tagService;
    public TagController(ITagService _tagService) 
    {
        tagService = _tagService;
    }
    [HttpPost("tag/add/{name}")]
    public async Task<IActionResult> AddTag(string tag)
    {
        bool result = await tagService.AddTagAsync(tag);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpPost("tag/{name}/book/{id}")]
    public async Task<IActionResult> AddTagToBook(string name, Guid id)
    {
        bool result = await tagService.AddTagToBookAsync(name, id);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpDelete("tag/{name}")]
    public async Task<IActionResult> DeleteTag(string name)
    {
        bool result = await tagService.DeleteTagAsync(name);

        if (!result)
            return BadRequest();

        return Ok();
    }
    [HttpGet("tags")]
    public async Task<IReadOnlyList<string>> GetAllAsync()
    {
        return await tagService.AllTagsAsync();
    }


}
*/
﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servises.Interfaces;
using Servises.Services;

namespace Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("/api")]
public class TagController(ITagService tagService) : Controller
{

    [HttpPost("tag/add/{name}")]
    public async Task<IActionResult> AddTag(string name)
    {
        await tagService.AddTagAsync(name);
        return Ok();
    }
    [HttpDelete("tag/{name}")]
    public async Task<IActionResult> DeleteTag(string name)
    {
        await tagService.DeleteTagAsync(name);
        return Ok();
    }
    [HttpGet("tags")]
    public async Task<IReadOnlyList<string>> GetAllAsync()
    {
        return await tagService.AllTagsAsync();
    }


}

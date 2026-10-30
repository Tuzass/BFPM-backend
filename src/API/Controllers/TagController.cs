using Infrastructure.Database;
using Domain.Entities;
using Services;
using Services.Exceptions;
using API.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using API.Responses;

namespace API.Controllers;

[ApiController]
[Route("api/tags")]
public class TagController: ControllerBase
{
    private readonly UserManager<User> _userManager = null!;

    public TagController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    private TagController()
    {
        
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTag(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            throw new InexistentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            var tag = await DomainService.GetTagByID(context, id);
            var project = await DomainService.GetProjectByID(context, tag.ProjectID);

            if (project.OwnerID != user.Id && !await DomainService.IsUserAssignedToProject(context, user.Id, id))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            TagResponse tagResponse = new(tag.ID, tag.ProjectID, tag.Name, tag.Version);

            return Ok(tagResponse);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateTag([FromBody] CreateTagRequest request)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            throw new InexistentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            Tag tag = new(request.ProjectID, request.Name, null);
            
            await DomainService.CreateTag(context, tag);
            await context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTag),
                new {id = tag.ID},
                new {tag.ID, tag.Name}
            );
        }
    }
}

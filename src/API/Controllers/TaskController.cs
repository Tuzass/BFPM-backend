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
[Route("api/tasks")]
public class TaskController: ControllerBase
{
    private readonly UserManager<User> _userManager = null!;

    public TaskController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    private TaskController()
    {
        
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTask(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            throw new InexistentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            var task = await DomainService.GetTaskByID(context, id);
            var project = await DomainService.GetProjectByID(context, task.ProjectID);

            if (project.OwnerID != user.Id && !await DomainService.IsUserAssignedToProject(context, user.Id, id))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            TaskResponse taskResponse = new(task.ID, task.ProjectID, task.Name, task.Description, task.Status, task.Priority, task.DueDate, task.CreatedAt, task.CompletedAt, task.Version);

            return Ok(taskResponse);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] CreateTaskRequest request)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            throw new InexistentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            TaskItem task = new(request.ProjectID, request.Name, request.Description, request.Status, request.Priority, request.DueDate, null, null, null);
            
            await DomainService.CreateTask(context, task);
            await context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetTask),
                new {id = task.ID},
                new {task.ID, task.Name}
            );
        }
    }
}

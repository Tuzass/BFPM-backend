using Infrastructure.Database;
using Domain.Entities;
using Services;
using Services.Exceptions;
using API.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Entities;
using API.Responses;

namespace API.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectController: ControllerBase
{
    private readonly UserManager<User> _userManager = null!;

    public ProjectController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    private ProjectController()
    {
        
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProject(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            throw new InexistentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            var project = await DomainService.GetProjectByID(context, id);

            if (project.OwnerID != user.Id && !await DomainService.IsUserAssignedToProject(context, user.Id, id))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            var tasks = await DomainService.GetTasksByProject(context, id);
            var tags = await DomainService.GetTagsByProject(context, id);
            var taskTagLinks = await DomainService.GetTaskTagLinksByProject(context, id);
            var assignedUsers = await DomainService.GetProjectMembers(context, id);

            List<TaskResponse> taskResponses = [];
            List<TagResponse> tagResponses = [];
            List<TaskTagLinkResponse> taskTagLinkResponses = [];
            List<UserAssignmentResponse> userAssignments = [];

            foreach (var task in tasks)
            {
                taskResponses.Add(new(task.ID, task.ProjectID, task.Name, task.Description, task.Status, task.Priority, task.DueDate, task.CreatedAt, task.CompletedAt, task.Version));
            }

            foreach (var tag in tags)
            {
                tagResponses.Add(new(tag.ID, tag.ProjectID, tag.Name, tag.Version));
            }

            foreach (var link in taskTagLinks)
            {
                taskTagLinkResponses.Add(new(link.TaskID, link.TagID));
            }

            foreach (var u in assignedUsers)
            {
                var role = await DomainService.GetUserRoleInProject(context, u.Id, id);
                userAssignments.Add(new(u.Id, u.UserName!, role));
            }

            ProjectResponse projectResponse = new(project.ID, project.Name, project.Description, user.Id, user.UserName!, project.Status, project.CreatedAt, project.CompletedAt, project.Version, taskResponses, tagResponses, taskTagLinkResponses, userAssignments);

            return Ok(projectResponse);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UploadProject([FromBody] UploadProjectRequest request)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            throw new InexistentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            Project project = new(user.Id, request.Name, request.Description, request.Status, null, null, null);

            List<TaskItem> tasks = [];
            for (int i = 0; i < request.Tasks.Count; i++)
            {
                var task = request.Tasks[i];
                tasks.Add(new(0, task.Name, task.Description, task.Status, task.Priority, task.DueDate, null, null, null));
            }

            List<Tag> tags = [];
            for (int i = 0; i < request.Tags.Count; i++)
            {
                var tag = request.Tags[i];
                tags.Add(new(0, tag.Name));
            }

            List<TaskTagLink> taskTagLinks = [];
            foreach (var link in request.TaskTagLinks)
            {
                taskTagLinks.Add(new(link.TaskID, link.TagID));
            }
            
            var projectID = await DomainService.UploadProject(context, project, tasks, tags, taskTagLinks);

            return CreatedAtAction(
                nameof(GetProject),
                new {id = projectID},
                new {project.ID, project.Name, project.OwnerID}
            );
        }
    }
}

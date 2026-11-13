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

    private async Task<ProjectResponse> GetProjectResponse(AppDbContext context, int userID, int projectID)
    {
        var project = await DomainService.GetProjectByID(context, projectID);

        if (project.OwnerID != userID && !await DomainService.IsUserAssignedToProject(context, userID, projectID))
        {
            throw new Services.Exceptions.UnauthorizedAccessException();
        }

        var tasks = await DomainService.GetTasksByProject(context, projectID);
        var tags = await DomainService.GetTagsByProject(context, projectID);
        var taskTagLinks = await DomainService.GetTaskTagLinksByProject(context, projectID);
        var assignedUsers = await DomainService.GetProjectMembers(context, projectID);

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
            var role = await DomainService.GetUserRoleInProject(context, u.Id, projectID);
            userAssignments.Add(new(u.Id, u.UserName!, role));
        }

        var owner = await UserService.GetUserByID(context, project.OwnerID);

        return new(project.ID, project.Name, project.Description, project.OwnerID, owner.UserName!, project.Status, project.CreatedAt, project.CompletedAt, project.Version, taskResponses, tagResponses, taskTagLinkResponses, userAssignments);
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
            var projectResponse = await GetProjectResponse(context, user.Id, id);
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

            try
            {   
                var projectID = await DomainService.CreateProject(context, project, tasks, tags, taskTagLinks);
                var projectResponse = await GetProjectResponse(context, user.Id, projectID);

                return CreatedAtAction(
                    nameof(GetProject),
                    new {id = projectID},
                    projectResponse
                );
            }

            catch (DuplicateProjectException)
            {
                RegisterConflictResponse registerConflictResponse = new(
                    "The user already has a project with this name.",
                    "project"
                );

                return Conflict(registerConflictResponse);
            }
        }
    }
}

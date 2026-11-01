using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Domain.Entities;
using Services;
using Services.Exceptions;
using API.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers;

[ApiController]
[Route("api/actions")]
public class ActionController: ControllerBase
{
    private readonly UserManager<User> _userManager = null!;

    public ActionController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    private ActionController()
    {
        
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAction(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            throw new InexistentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            var userAction = await WorkflowEngine.GetUserActionByID(context, id);

            if (user.Id != userAction.UserID)
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            return Ok(new
            {
                userAction.ActionType,
                userAction.CreatedAt,
                userAction.Status,
                userAction.ProcessedAt
            });
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateAction([FromBody] CreateActionRequest request)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            throw new InexistentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            var project = await DomainService.GetProjectByID(context, request.ProjectID);

            if (project.OwnerID != user.Id
             && DomainService.RequiresOwnerPrivileges(request.ActionType, request.TargetType))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            if (!await DomainService.IsUserProjectManager(context, user.Id, project.ID)
             && DomainService.RequiresManagerPrivileges(request.ActionType, request.TargetType))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            if (!await DomainService.IsUserAssignedToProject(context, user.Id, project.ID))
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            UserAction userAction = new(user.Id, request.ProjectID, request.ActionType, request.TargetType, request.RelatedEntityType, request.TargetID, request.RelatedEntityID, request.Change);
            var issuedActions = await WorkflowEngine.GenerateIssuedActionsFromUserAction(context, userAction);
            var entityIdPairs = WorkflowEngine.ConstructEntityIdPairs(issuedActions);

            context.UserActions.Add(userAction);
            context.IssuedActions.AddRange(issuedActions);
            await context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(SendVersions),
                new {id = userAction.ID},
                new {affected_entities = entityIdPairs}
            );
        }
    }

    [Authorize]
    [HttpPost("{id:int}/versions")]
    public async Task<IActionResult> SendVersions(int id, [FromBody] SendVersionsRequest request){
        var user = await _userManager.GetUserAsync(User);

        if (user is null)
        {
            throw new InexistentUserException();
        }

        using (var context = AppDbContextFactory.CreateContext())
        {
            var userAction = await WorkflowEngine.GetUserActionByID(context, id);

            if (user.Id != userAction.UserID)
            {
                throw new Services.Exceptions.UnauthorizedAccessException();
            }

            if (userAction.Status != UserActionStatus.Processing)
            {
                return Ok(userAction.Status);
            }

            var issuedActions = await WorkflowEngine.GetIssuedActionsByUserAction(context, userAction.ID);

            if (!await WorkflowEngine.ValidateIssuedActionSet(context, issuedActions, request.Versions))
            {
                userAction.Reject();
                context.IssuedActions.RemoveRange(issuedActions);
                await context.SaveChangesAsync();
                
                return Conflict();
            }

            await WorkflowEngine.ExecuteIssuedActions(context, issuedActions);
            userAction.Approve();
            await context.SaveChangesAsync();

            return Ok(userAction.Status);
        }
    }
}

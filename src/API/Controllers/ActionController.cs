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

        var ct = HttpContext.RequestAborted;

        while (true)
        {
            await ReservationService.WaitForAvailability(request.ProjectID, ct);

            using (var context = AppDbContextFactory.CreateContext())
            {
                using (var transaction = await context.Database.BeginTransactionAsync(ct))
                {
                    if (!await ReservationService.TryAcquireProjectLock(context, request.ProjectID, ct))
                    {
                        await transaction.RollbackAsync(ct);
                        await Task.Delay(1000, ct);
                        continue;
                    }

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

                    context.UserActions.Add(userAction);
                    await context.SaveChangesAsync(ct);

                    if (!await ReservationService.TryCreateReservation(context, project.ID, userAction.ID, ct))
                    {
                        await transaction.RollbackAsync(ct);
                        continue;
                    }

                    var issuedActions = await WorkflowEngine.GenerateIssuedActionsFromUserAction(context, userAction);
                    var entityIdPairs = WorkflowEngine.ConstructEntityIdPairs(issuedActions);
                    context.IssuedActions.AddRange(issuedActions);

                    await context.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    return CreatedAtAction(
                        nameof(SendVersions),
                        new {id = userAction.ID},
                        new {affected_entities = entityIdPairs}
                    );
                }
            }
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

        var ct = HttpContext.RequestAborted;

        using (var context = AppDbContextFactory.CreateContext())
        {
            using (var transaction = await context.Database.BeginTransactionAsync(ct))
            {
                var userAction = await WorkflowEngine.GetUserActionByID(context, id);

                if (user.Id != userAction.UserID)
                {
                    throw new Services.Exceptions.UnauthorizedAccessException();
                }

                var ownsReservation = await ReservationService.LockReservedProject(context, userAction.ProjectID, userAction.ID, ct);
                await context.Entry(userAction).ReloadAsync(ct);

                if (userAction.Status != UserActionStatus.Processing)
                {
                    await transaction.CommitAsync(ct);
                    return Ok(userAction.Status);
                }

                if (!ownsReservation)
                {
                    throw new InvalidOperationException("User action doesn't own the project's reservation");
                }

                var issuedActions = await WorkflowEngine.GetIssuedActionsByUserAction(context, userAction.ID);

                if (!await WorkflowEngine.ValidateIssuedActionSet(context, issuedActions, request.Versions))
                {
                    userAction.Reject();
                    context.IssuedActions.RemoveRange(issuedActions);

                    await ReservationService.ReleaseProjectReservation(context, userAction.ProjectID, userAction.ID, ct);
                    await context.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);
                    
                    return Conflict();
                }

                await WorkflowEngine.ExecuteIssuedActions(context, issuedActions);
                userAction.Approve();

                await ReservationService.ReleaseProjectReservation(context, userAction.ProjectID, userAction.ID, ct);
                await context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return Ok(userAction.Status);
            }
        }
    }
}

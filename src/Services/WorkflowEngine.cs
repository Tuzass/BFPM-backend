using Domain.Enums;
using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Services.Exceptions;
using Workflow.Entities;
using Workflow.Enums;

namespace Services;

static public class WorkflowEngine
{
    public static async Task CreateWorkflowRules()
    {
        List<SystemAction> systemActions = [
            new(1, ActionType.LogActivity, ActionTarget.None, State.None),

            new(2, ActionType.SendNotification, ActionTarget.RelatedUser, State.None),
            new(3, ActionType.SendNotification, ActionTarget.UsersAssignedToProject, State.None),
            new(4, ActionType.SendNotification, ActionTarget.UsersAssignedToTask, State.None),
            new(5, ActionType.SendNotification, ActionTarget.ProjectManagers, State.None),
            new(6, ActionType.SendNotification, ActionTarget.AffectedProjectManagers, State.None),

            new(7, ActionType.ChangeAttribute, ActionTarget.RelatedTask, State.TaskStatusOnHold),
            new(8, ActionType.ChangeAttribute, ActionTarget.RelatedTasks, State.TaskStatusCompleted),
            new(9, ActionType.ChangeAttribute, ActionTarget.InProgressTasks, State.TaskStatusOnHold),
            new(10, ActionType.ChangeAttribute, ActionTarget.OnHoldProject, State.ProjectStatusActive),
            new(11, ActionType.ChangeAttribute, ActionTarget.ActiveProject, State.ProjectStatusOnHold),

            new(12, ActionType.UnassignUser, ActionTarget.AssignedTasks, State.None),
            new(13, ActionType.UnassignUser, ActionTarget.AssignedProjects, State.None),
            new(14, ActionType.UnassignUsers, ActionTarget.UsersAssignedToTask, State.None)
        ];

        List<WorkflowRule> workflowRules = [
            new(1, Trigger.UserAssignmentRoleChanged, State.None),
            new(2, Trigger.UserAssignedToProject, State.None),
            new(3, Trigger.UserUnassignedFromProject, State.None),

            new(4, Trigger.UserAssignedToTask, State.None),
            new(5, Trigger.UserUnassignedFromTask, State.None),

            new(6, Trigger.UserUnassignedFromTask, State.NoUsersAssignedToInProgressTask),

            new(7, Trigger.TagCreated, State.None),
            new(8, Trigger.TagNameChanged, State.None),
            new(9, Trigger.TagDeleted, State.None),

            new(10, Trigger.TagAssignedToTask, State.None),
            new(11, Trigger.TagUnassignedFromTask, State.None),

            new(12, Trigger.TaskCreated, State.None),

            new(13, Trigger.TaskDeleted, State.None),
            new(14, Trigger.TaskNameChanged, State.None),
            new(15, Trigger.TaskDescriptionChanged, State.None),
            new(16, Trigger.TaskDueDateChanged, State.None),
            new(17, Trigger.TaskStatusChanged, State.TaskStatusTodo),
            new(18, Trigger.TaskStatusChanged, State.TaskStatusInProgress),
            new(19, Trigger.TaskStatusChanged, State.TaskStatusOnHold),
            new(20, Trigger.TaskStatusChanged, State.TaskStatusBlocked),
            new(21, Trigger.TaskStatusChanged, State.TaskStatusCompleted),
            new(22, Trigger.TaskPriorityChanged, State.TaskPriorityLow),
            new(23, Trigger.TaskPriorityChanged, State.TaskPriorityMedium),
            new(24, Trigger.TaskPriorityChanged, State.TaskPriorityHigh),
            new(25, Trigger.TaskPriorityChanged, State.TaskPriorityUrgent),

            new(26, Trigger.TaskCreated, State.TasksInProgress),
            new(27, Trigger.TaskStatusChanged, State.TasksInProgress),

            new(28, Trigger.TaskStatusChanged, State.NoTasksInProgress),
            new(29, Trigger.TaskDeleted, State.NoTasksInProgress),

            new(30, Trigger.TaskStatusChanged, State.UsersAssignedToCompletedTask),

            new(31, Trigger.ProjectUploaded, State.None),

            new(32, Trigger.ProjectDeleted, State.None),

            new(33, Trigger.ProjectNameChanged, State.None),
            new(34, Trigger.ProjectDescriptionChanged, State.None),
            new(35, Trigger.ProjectStatusChanged, State.ProjectStatusActive),

            new(36, Trigger.ProjectStatusChanged, State.ProjectStatusOnHold),
            new(37, Trigger.ProjectStatusChanged, State.ProjectStatusOnHold),

            new(38, Trigger.ProjectStatusChanged, State.ProjectStatusCompleted)
        ];

        using (var context = AppDbContextFactory.CreateContext())
        {
            var currentActions = await context.SystemActions.ToListAsync();
            var currentRules = await context.WorkflowRules.ToListAsync();

            if (currentActions.Count > 0 || currentActions.Count > 0)
            {
                return;
            }

            context.SystemActions.AddRange(systemActions);
            context.WorkflowRules.AddRange(workflowRules);
            await context.SaveChangesAsync();
        }
    }

    public static async Task CreateSystemActionPreferences(int projectID)
    {
        List<SystemActionPreference> systemActionPreferences = [
            new(1, 1, projectID),
            new(1, 2, projectID),
            new(1, 3, projectID),
            new(1, 5, projectID),

            new(2, 1, projectID),
            new(2, 2, projectID),
            new(2, 3, projectID),
            new(2, 5, projectID),

            new(3, 1, projectID),
            new(3, 2, projectID),
            new(3, 3, projectID),
            new(3, 5, projectID),

            new(4, 1, projectID),
            new(4, 2, projectID),
            new(4, 4, projectID),
            new(4, 5, projectID),

            new(5, 1, projectID),
            new(5, 2, projectID),
            new(5, 4, projectID),
            new(5, 5, projectID),

            new(6, 1, projectID),
            new(6, 2, projectID),
            new(6, 5, projectID),
            new(6, 7, projectID),

            new(7, 1, projectID),
            new(7, 3, projectID),
            new(7, 5, projectID),

            new(8, 1, projectID),
            new(8, 3, projectID),
            new(8, 5, projectID),

            new(9, 1, projectID),
            new(9, 3, projectID),
            new(9, 5, projectID),

            new(10, 1, projectID),
            new(10, 4, projectID),
            new(10, 5, projectID),

            new(11, 1, projectID),
            new(11, 4, projectID),
            new(11, 5, projectID),

            new(12, 1, projectID),
            new(12, 5, projectID),

            new(13, 1, projectID),
            new(13, 4, projectID),
            new(13, 5, projectID),

            new(14, 1, projectID),
            new(14, 4, projectID),
            new(14, 5, projectID),

            new(15, 1, projectID),
            new(15, 4, projectID),
            new(15, 5, projectID),

            new(16, 1, projectID),
            new(16, 4, projectID),
            new(16, 5, projectID),

            new(17, 1, projectID),
            new(17, 4, projectID),
            new(17, 5, projectID),

            new(18, 1, projectID),
            new(18, 4, projectID),
            new(18, 5, projectID),

            new(19, 1, projectID),
            new(19, 4, projectID),
            new(19, 5, projectID),

            new(20, 1, projectID),
            new(20, 4, projectID),
            new(20, 5, projectID),

            new(21, 1, projectID),
            new(21, 4, projectID),
            new(21, 5, projectID),

            new(22, 1, projectID),
            new(22, 4, projectID),
            new(22, 5, projectID),

            new(23, 1, projectID),
            new(23, 4, projectID),
            new(23, 5, projectID),

            new(24, 1, projectID),
            new(24, 4, projectID),
            new(24, 5, projectID),

            new(25, 1, projectID),
            new(25, 4, projectID),
            new(25, 5, projectID),

            new(26, 1, projectID),
            new(26, 4, projectID),
            new(26, 5, projectID),
            new(26, 10, projectID),

            new(27, 1, projectID),
            new(27, 4, projectID),
            new(27, 5, projectID),
            new(27, 10, projectID),

            new(28, 1, projectID),
            new(28, 4, projectID),
            new(28, 5, projectID),
            new(28, 11, projectID),

            new(29, 1, projectID),
            new(29, 4, projectID),
            new(29, 5, projectID),
            new(29, 11, projectID),

            new(30, 1, projectID),
            new(30, 4, projectID),
            new(30, 5, projectID),
            new(30, 14, projectID),

            new(31, 1, projectID),
            new(31, 5, projectID),

            new(32, 1, projectID),
            new(32, 4, projectID),
            new(32, 5, projectID),

            new(33, 1, projectID),
            new(33, 4, projectID),
            new(33, 5, projectID),

            new(34, 1, projectID),
            new(34, 4, projectID),
            new(34, 5, projectID),

            new(35, 1, projectID),
            new(35, 4, projectID),
            new(35, 5, projectID),

            new(36, 1, projectID),
            new(36, 3, projectID),
            new(36, 5, projectID),
            new(36, 9, projectID),

            new(37, 1, projectID),
            new(37, 3, projectID),
            new(37, 5, projectID),
            new(37, 9, projectID),

            new(38, 1, projectID),
            new(38, 3, projectID),
            new(38, 5, projectID),
            new(38, 8, projectID)
        ];

        using (var context = AppDbContextFactory.CreateContext())
        {
            context.SystemActionPreferences.AddRange(systemActionPreferences);
            await context.SaveChangesAsync();
        }
    }

    public static async Task<List<WorkflowRule>> GetWorkflowRules()
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.WorkflowRules.ToListAsync();
        }
    }

    public static async Task<List<SystemAction>> GetSystemActions()
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.SystemActions.ToListAsync();
        }
    }

    public static async Task<List<SystemActionPreference>> GetSystemActionPreferences(int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.SystemActionPreferences
                .Where(sap => sap.ProjectID == projectID)
                .ToListAsync();
        }
    }

    public static async Task ChangeSystemActionPreference(int ruleID, int actionID, int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var systemActionPreference = await context.SystemActionPreferences
                .SingleOrDefaultAsync(sap => sap.RuleID == ruleID && sap.ActionID == actionID && sap.ProjectID == projectID);
            
            if (systemActionPreference is null)
            {
                throw new InexistentSystemActionPreferenceException();
            }
            
            systemActionPreference.Enabled = !systemActionPreference.Enabled;
            await context.SaveChangesAsync();
        }
    }

    // methods that evaluate State

    public static async Task<List<SystemAction>> Propagate(int projectID, Trigger trigger, State state)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.WorkflowRules
                .Where(wr => wr.Trigger == trigger && wr.State == state)
                .Join(
                    context.SystemActionPreferences,
                    wr => wr.ID,
                    sap => sap.RuleID,
                    (wr, sap) => sap)
                .Where(sap => sap.ProjectID == projectID && sap.Enabled)
                .Join(
                    context.SystemActions,
                    sap => sap.ActionID,
                    sa => sa.ID,
                    (sap, sa) => sa)
                .ToListAsync();
        }
    }

    private static Trigger GetTriggerFromActionType(ActionType actionType, DomainEntityType? entityType, DomainEntityType? relatedEntityType)
    {
        switch (actionType)
        {
            case ActionType.AssignUser:
                return relatedEntityType switch
                {
                    DomainEntityType.Project => Trigger.UserAssignedToProject,
                    DomainEntityType.Task => Trigger.UserAssignedToTask,
                    _ => throw new UnexpectedEntityTypeException(),
                };
            
            case ActionType.ChangeUserRole:
                return Trigger.UserAssignmentRoleChanged;

            case ActionType.CreateEntity:
                return entityType switch
                {
                    DomainEntityType.Task => Trigger.TaskCreated,
                    DomainEntityType.Tag => Trigger.TagCreated,
                    _ => throw new UnexpectedEntityTypeException(),
                };
            
            case ActionType.DeleteEntity:
                return entityType switch
                {
                    DomainEntityType.Project => Trigger.ProjectDeleted,
                    DomainEntityType.Task => Trigger.TaskDeleted,
                    DomainEntityType.Tag => Trigger.TagDeleted,
                    _ => throw new UnexpectedEntityTypeException(),
                };
            
            case ActionType.UnassignUser:
                return relatedEntityType switch
                {
                    DomainEntityType.Project => Trigger.UserUnassignedFromProject,
                    DomainEntityType.Task => Trigger.UserUnassignedFromTask,
                    _ => throw new UnexpectedEntityTypeException(),
                };
            
            case ActionType.UploadProject:
                return Trigger.ProjectUploaded;
            
            default:
                throw new UnexpectedActionTypeException();
        }
    }

    private static List<Trigger> GetTriggersFromAttributeChange(DomainEntityType entityType, LocalChange localChange)
    {
        List<Trigger> triggers = [];

        switch (entityType)
        {
            case DomainEntityType.Tag:
                return [Trigger.TagNameChanged];
            
            case DomainEntityType.Task:
                if (localChange.Name is not null)
                {
                    triggers.Add(Trigger.TaskNameChanged);
                }

                if (localChange.Description is not null)
                {
                    triggers.Add(Trigger.TaskDescriptionChanged);
                }

                if (localChange.Status is not null)
                {
                    triggers.Add(Trigger.TaskStatusChanged);
                }

                if (localChange.Priority is not null)
                {
                    triggers.Add(Trigger.TaskPriorityChanged);
                }

                if (localChange.DueDateChanged)
                {
                    triggers.Add(Trigger.TaskDueDateChanged);
                }

                return triggers;
            
            case DomainEntityType.Project:
                if (localChange.Name is not null)
                {
                    triggers.Add(Trigger.ProjectNameChanged);
                }

                if (localChange.Description is not null)
                {
                    triggers.Add(Trigger.ProjectDescriptionChanged);
                }

                if (localChange.Status is not null)
                {
                    triggers.Add(Trigger.ProjectStatusChanged);
                }

                return triggers;
            
            default:
                throw new UnexpectedEntityTypeException();
        }
    }

    public static async Task<List<IssuedAction>> GenerateIssuedActionSet(UserAction userAction)
    {
        // returns the list of issued actions that are generated by propagating userAction
        return [];
    }

    public static async Task<bool> ValidateIssuedActionSet(List<IssuedAction> issuedActions)
    {
        // validates the set of issued actions against the user's local DB
        return true;
    }

    public static async Task ExecuteIssuedActions()
    {
        // executes all issued actions that are currently marked as "pending" in the DB
        // the actions were persisted by whoever calls ValidateActionSet, if they are valid
    }
}

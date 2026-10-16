using Domain.Enums;
using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
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
            new(8, Trigger.TagDeleted, State.None),

            new(9, Trigger.TagAssignedToTask, State.None),
            new(10, Trigger.TagUnassignedFromTask, State.None),

            new(11, Trigger.TaskCreated, State.None),

            new(12, Trigger.TaskDeleted, State.None),
            new(13, Trigger.TaskNameChanged, State.None),
            new(14, Trigger.TaskDescriptionChanged, State.None),
            new(15, Trigger.TaskDueDateChanged, State.None),
            new(16, Trigger.TaskStatusChanged, State.TaskStatusTodo),
            new(17, Trigger.TaskStatusChanged, State.TaskStatusInProgress),
            new(18, Trigger.TaskStatusChanged, State.TaskStatusOnHold),
            new(19, Trigger.TaskStatusChanged, State.TaskStatusBlocked),
            new(20, Trigger.TaskStatusChanged, State.TaskStatusCompleted),
            new(21, Trigger.TaskPriorityChanged, State.TaskPriorityLow),
            new(22, Trigger.TaskPriorityChanged, State.TaskPriorityMedium),
            new(23, Trigger.TaskPriorityChanged, State.TaskPriorityHigh),
            new(24, Trigger.TaskPriorityChanged, State.TaskPriorityUrgent),

            new(25, Trigger.TaskCreated, State.TasksInProgress),
            new(26, Trigger.TaskStatusChanged, State.TasksInProgress),

            new(27, Trigger.TaskStatusChanged, State.NoTasksInProgress),
            new(28, Trigger.TaskDeleted, State.NoTasksInProgress),

            new(29, Trigger.TaskStatusChanged, State.UsersAssignedToCompletedTask),

            new(30, Trigger.ProjectUploaded, State.None),

            new(31, Trigger.ProjectDeleted, State.None),

            new(32, Trigger.ProjectNameChanged, State.None),
            new(33, Trigger.ProjectDescriptionChanged, State.None),
            new(34, Trigger.ProjectStatusChanged, State.ProjectStatusActive),

            new(35, Trigger.ProjectStatusChanged, State.ProjectStatusOnHold),
            new(36, Trigger.ProjectStatusChanged, State.ProjectStatusOnHold),

            new(37, Trigger.ProjectStatusChanged, State.ProjectStatusCompleted)
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
            new(9, 4, projectID),
            new(9, 5, projectID),

            new(10, 1, projectID),
            new(10, 4, projectID),
            new(10, 5, projectID),

            new(11, 1, projectID),
            new(11, 5, projectID),

            new(12, 1, projectID),
            new(12, 4, projectID),
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
            new(25, 10, projectID),

            new(26, 1, projectID),
            new(26, 4, projectID),
            new(26, 5, projectID),
            new(26, 10, projectID),

            new(27, 1, projectID),
            new(27, 4, projectID),
            new(27, 5, projectID),
            new(27, 11, projectID),

            new(28, 1, projectID),
            new(28, 4, projectID),
            new(28, 5, projectID),
            new(28, 11, projectID),

            new(29, 1, projectID),
            new(29, 4, projectID),
            new(29, 5, projectID),
            new(29, 14, projectID),

            new(30, 1, projectID),
            new(30, 5, projectID),

            new(31, 1, projectID),
            new(31, 4, projectID),
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
            new(35, 3, projectID),
            new(35, 5, projectID),
            new(35, 9, projectID),

            new(36, 1, projectID),
            new(36, 3, projectID),
            new(36, 5, projectID),
            new(36, 9, projectID),

            new(37, 1, projectID),
            new(37, 3, projectID),
            new(37, 5, projectID),
            new(37, 8, projectID)
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

    public static async Task<List<IssuedAction>> GenerateIssuedActions(SystemAction systemAction)
    {
        // returns the list of issued actions that are generated by propagating systemAction
        return [];
    }

    public static async Task<List<IssuedAction>> Propagate(UserAction userAction)
    {
        // returns the list of issued actions that are generated by propagating userAction
        return [];
    }

    public static async Task<bool> ValidateActionSet(List<IssuedAction> issuedActions)
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

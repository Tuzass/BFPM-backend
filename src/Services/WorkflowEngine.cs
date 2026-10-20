using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using Services.Exceptions;
using Workflow.Entities;
using Workflow.Enums;

namespace Services;

public static class WorkflowEngine
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

            new(12, ActionType.UnassignUsers, ActionTarget.UsersAssignedToTask, State.None)
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
            new(6, 4, projectID),
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
            new(30, 12, projectID),

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

    private static List<Trigger> GetTriggersFromActionType(ActionType actionType, DomainEntityType entityType, DomainEntityType? relatedEntityType, Change? change)
    {
        List<Trigger> triggers = [];

        switch (actionType)
        {
            case ActionType.LogActivity:
            case ActionType.SendNotification:
                return triggers;

            case ActionType.AssignUser:
                triggers.Add(relatedEntityType switch
                {
                    DomainEntityType.Project => Trigger.UserAssignedToProject,
                    DomainEntityType.Task => Trigger.UserAssignedToTask,
                    _ => throw new UnexpectedEntityTypeException(),
                });

                return triggers;
            
            case ActionType.ChangeUserRole:
                triggers.Add(Trigger.UserAssignmentRoleChanged);
                return triggers;

            case ActionType.CreateEntity:
                triggers.Add(relatedEntityType switch
                {
                    DomainEntityType.Task => Trigger.TaskCreated,
                    DomainEntityType.Tag => Trigger.TagCreated,
                    _ => throw new UnexpectedEntityTypeException(),
                });

                return triggers;
            
            case ActionType.DeleteEntity:
                triggers.Add(relatedEntityType switch
                {
                    DomainEntityType.Project => Trigger.ProjectDeleted,
                    DomainEntityType.Task => Trigger.TaskDeleted,
                    DomainEntityType.Tag => Trigger.TagDeleted,
                    _ => throw new UnexpectedEntityTypeException(),
                });

                return triggers;
            
            case ActionType.UnassignUser:
                triggers.Add(relatedEntityType switch
                {
                    DomainEntityType.Project => Trigger.UserUnassignedFromProject,
                    DomainEntityType.Task => Trigger.UserUnassignedFromTask,
                    _ => throw new UnexpectedEntityTypeException(),
                });

                return triggers;
            
            case ActionType.UploadProject:
                triggers.Add(Trigger.ProjectUploaded);
                return triggers;
            
            case ActionType.ChangeAttribute:
                if (change is null)
                {
                    throw new NullReferenceException();
                }

                switch (entityType)
                {
                    case DomainEntityType.Tag:
                        return [Trigger.TagNameChanged];
                    
                    case DomainEntityType.Task:
                        if (change.Name is not null)
                        {
                            triggers.Add(Trigger.TaskNameChanged);
                        }

                        if (change.Description is not null)
                        {
                            triggers.Add(Trigger.TaskDescriptionChanged);
                        }

                        if (change.Status is not null)
                        {
                            triggers.Add(Trigger.TaskStatusChanged);
                        }

                        if (change.Priority is not null)
                        {
                            triggers.Add(Trigger.TaskPriorityChanged);
                        }

                        if (change.DueDateChanged)
                        {
                            triggers.Add(Trigger.TaskDueDateChanged);
                        }

                        return triggers;
                    
                    case DomainEntityType.Project:
                        if (change.Name is not null)
                        {
                            triggers.Add(Trigger.ProjectNameChanged);
                        }

                        if (change.Description is not null)
                        {
                            triggers.Add(Trigger.ProjectDescriptionChanged);
                        }

                        if (change.Status is not null)
                        {
                            triggers.Add(Trigger.ProjectStatusChanged);
                        }

                        return triggers;
                    
                    default:
                        throw new UnexpectedEntityTypeException();
                }
            
            default:
                throw new UnexpectedActionTypeException();
        }
    }

    private static async Task<bool> EvaluateNoUsersAssignedToInProgressTask(int taskID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var assignedUsers = await DomainService.GetUsersAssignedToTask(context, taskID);

            if (assignedUsers.Count > 1)
            {
                return false;
            }

            return true;
        }
    }

    private static async Task<bool> EvaluateUsersAssignedToCompletedTask(int taskID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var assignedUsers = await DomainService.GetUsersAssignedToTask(context, taskID);

            if (assignedUsers.Count > 0)
            {
                return false;
            }

            return true;
        }
    }

    private static async Task<bool> EvaluateNoTasksInProgress(int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var tasks = (await DomainService.GetTasksByProject(context, projectID))
                            .Where(t => t.Status == TaskItemStatus.InProgress).ToList();

            if (tasks.Count > 1)
            {
                return false;
            }

            return true;
        }
    }

    private static async Task<bool> EvaluateTasksInProgress(int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var tasks = (await DomainService.GetTasksByProject(context, projectID))
                            .Where(t => t.Status == TaskItemStatus.InProgress).ToList();

            if (tasks.Count > 0)
            {
                return false;
            }

            return true;
        }
    }

    private static async Task<List<State>> GetStatesFromTaskStatusChange(int targetID, Change change)
    {
        TaskItem task;
        List<State> states = [];

        using (var context = AppDbContextFactory.CreateContext())
        {
            task = await DomainService.GetTaskByID(context, targetID);
        }

        if (change is null)
        {
            throw new NullJsonException();
        }

        if (task.Status != TaskItemStatus.Todo
        && change.Status == DomainEntityStatus.Todo)
        {
            states.Add(State.TaskStatusTodo);
        }

        if (task.Status != TaskItemStatus.InProgress
        && change.Status == DomainEntityStatus.InProgress)
        {
            if (await EvaluateTasksInProgress(task.ProjectID))
            {
                states.Add(State.TasksInProgress);
            }

            states.Add(State.TaskStatusInProgress);
        }

        if (task.Status == TaskItemStatus.InProgress
        && change.Status != DomainEntityStatus.InProgress
        && await EvaluateNoTasksInProgress(task.ProjectID))
        {
            states.Add(State.NoTasksInProgress);
        }

        if (task.Status != TaskItemStatus.OnHold
        && change.Status == DomainEntityStatus.OnHold)
        {
            states.Add(State.TaskStatusBlocked);
        }

        if (task.Status != TaskItemStatus.Blocked
        && change.Status == DomainEntityStatus.Blocked)
        {
            states.Add(State.TaskStatusBlocked);
        }

        if (task.Status != TaskItemStatus.Completed
        && change.Status == DomainEntityStatus.Completed)
        {
            if (await EvaluateUsersAssignedToCompletedTask(task.ProjectID))
            {
                states.Add(State.UsersAssignedToCompletedTask);
            }

            states.Add(State.TaskStatusInProgress);
        }

        return states;
    }

    private static async Task<List<State>> GetStatesFromProjectStatusChange(int targetID, Change change)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {   
            var project = await DomainService.GetProjectByID(context, targetID);
            List<State> states = [];

            if (project.Status != ProjectStatus.Active
            && change.Status == DomainEntityStatus.Active)
            {
                states.Add(State.ProjectStatusActive);
            }

            if (project.Status != ProjectStatus.OnHold
            && change.Status == DomainEntityStatus.OnHold)
            {
                states.Add(State.ProjectStatusOnHold);
            }

            if (project.Status != ProjectStatus.Archived
            && change.Status == DomainEntityStatus.Archived)
            {
                states.Add(State.ProjectStatusArchived);
            }

            if (project.Status != ProjectStatus.Completed
            && change.Status == DomainEntityStatus.Completed)
            {
                states.Add(State.ProjectStatusCompleted);
            }

            return states;
        }
    }

    private static async Task<List<State>> GetStatesFromTriggerAndAction(Trigger trigger, int targetID, int? relatedEntityID, Change? change)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            List<State> states = [];

            switch (trigger)
            {
                case Trigger.UserUnassignedFromTask:
                    if (relatedEntityID is null)
                    {
                        throw new UnexpectedEntityTypeException();
                    }

                    if (await EvaluateNoUsersAssignedToInProgressTask(relatedEntityID.Value))
                    {
                        states.Add(State.NoUsersAssignedToInProgressTask);
                    }

                    break;
                
                case Trigger.TaskCreated:
                    var task = await DomainService.GetTaskByID(context, targetID);

                    if (await EvaluateTasksInProgress(task.ProjectID))
                    {
                        states.Add(State.TasksInProgress);
                    }

                    break;
                
                case Trigger.TaskDeleted:
                    task = await DomainService.GetTaskByID(context, targetID);

                    if (await EvaluateNoTasksInProgress(task.ProjectID))
                    {
                        states.Add(State.NoTasksInProgress);
                    }

                    break;
                
                case Trigger.TaskStatusChanged:
                    if (change is null)
                    {
                        throw new NullReferenceException();
                    }

                    states = await GetStatesFromTaskStatusChange(targetID, change);
                    break;
                
                case Trigger.ProjectStatusChanged:
                    if (change is null)
                    {
                        throw new NullReferenceException();
                    }

                    states = await GetStatesFromProjectStatusChange(targetID, change);
                    break;
            }

            return states;
        }
    }

    private static async Task<List<IssuedAction>> GenerateIssuedActionsFromSystemAction(Trigger trigger, UserAction userAction, SystemAction systemAction, int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            List<IssuedAction> issuedActions = [];

            switch (systemAction.Type)
            {
                case ActionType.ChangeAttribute:
                    switch (systemAction.Target)
                    {
                        case ActionTarget.RelatedTask:
                            var status = systemAction.NewState switch
                            {
                                State.TaskStatusTodo => DomainEntityStatus.Todo,
                                State.TaskStatusInProgress => DomainEntityStatus.InProgress,
                                State.TaskStatusOnHold => DomainEntityStatus.OnHold,
                                State.TaskStatusBlocked => DomainEntityStatus.InProgress,
                                State.TaskStatusCompleted => DomainEntityStatus.Completed,
                                _ => DomainEntityStatus.Todo
                            };

                            if (userAction.RelatedEntityID is null)
                            {
                                throw new NullReferenceException();
                            }

                            Change change = new(DomainEntityType.Task, userAction.RelatedEntityID.Value, null, null, status, null, null, false, null);
                            issuedActions.Add(new(trigger,
                                            systemAction.Type,
                                            DomainEntityType.Task,
                                            null,
                                            userAction.RelatedEntityID.Value,
                                            null,
                                            change));
                            
                            break;
                        
                        case ActionTarget.RelatedTasks:
                            var tasks = await DomainService.GetTasksByProject(context, projectID);

                            foreach (var task in tasks)
                            {
                                status = systemAction.NewState switch
                                {
                                    State.TaskStatusTodo => DomainEntityStatus.Todo,
                                    State.TaskStatusInProgress => DomainEntityStatus.InProgress,
                                    State.TaskStatusOnHold => DomainEntityStatus.OnHold,
                                    State.TaskStatusBlocked => DomainEntityStatus.InProgress,
                                    State.TaskStatusCompleted => DomainEntityStatus.Completed,
                                    _ => DomainEntityStatus.Todo
                                };

                                change = new(DomainEntityType.Task, task.ID, null, null, status, null, null, false, null);
                                issuedActions.Add(new(trigger,
                                                systemAction.Type,
                                                DomainEntityType.Task,
                                                null,
                                                task.ID,
                                                null,
                                                change));
                            }
                            
                            break;
                        
                        case ActionTarget.InProgressTasks:
                            tasks = await DomainService.GetTasksByProject(context, projectID);
                            tasks = tasks.Where(t => t.Status == TaskItemStatus.InProgress).ToList();

                            foreach (var task in tasks)
                            {
                                status = systemAction.NewState switch
                                {
                                    State.TaskStatusTodo => DomainEntityStatus.Todo,
                                    State.TaskStatusInProgress => DomainEntityStatus.InProgress,
                                    State.TaskStatusOnHold => DomainEntityStatus.OnHold,
                                    State.TaskStatusBlocked => DomainEntityStatus.InProgress,
                                    State.TaskStatusCompleted => DomainEntityStatus.Completed,
                                    _ => DomainEntityStatus.Todo
                                };

                                change = new(DomainEntityType.Task, task.ID, null, null, status, null, null, false, null);
                                issuedActions.Add(new(trigger,
                                                systemAction.Type,
                                                DomainEntityType.Task,
                                                null,
                                                task.ID,
                                                null,
                                                change));
                            }
                            
                            break;
                        
                        default:
                            throw new UnexpectedActionTargetException();
                    }

                    break;

                case ActionType.UnassignUsers:
                    var users = await DomainService.GetUsersAssignedToTask(context, userAction.TargetID);

                    foreach (var user in users)
                    {
                        issuedActions.Add(new(trigger,
                                            systemAction.Type,
                                            DomainEntityType.Task,
                                            DomainEntityType.User,
                                            userAction.TargetID,
                                            user.ID,
                                            null));
                    }

                    break;
                
                default:
                    throw new UnexpectedActionTypeException();
            }

            return issuedActions;
        }
    }

    public static async Task<List<IssuedAction>> GenerateIssuedActionsFromUserAction(UserAction userAction, int projectID)
    {
        List<IssuedAction> issuedActions = [];
        List<Trigger> triggers = [];

        issuedActions.Add(new(Trigger.UserAction,
                              userAction.ActionType,
                              userAction.TargetType,
                              userAction.RelatedEntityType,
                              userAction.TargetID,
                              userAction.RelatedEntityID,
                              userAction.Change));

        triggers.AddRange(GetTriggersFromActionType(userAction.ActionType, userAction.TargetType, userAction.RelatedEntityType, userAction.Change));

        int i = 0;
        while (true)
        {
            var trigger = triggers[i];
            var states = await GetStatesFromTriggerAndAction(trigger, userAction.TargetID, userAction.RelatedEntityID, userAction.Change);
            
            if (states.Count == 0)
            {
                states = [State.None];
            }

            foreach (var state in states)
            {
                var systemActions = await Propagate(projectID, trigger, state);

                foreach (var systemAction in systemActions)
                {
                    triggers.AddRange(GetTriggersFromActionType(systemAction.Type, userAction.TargetType, userAction.RelatedEntityType, userAction.Change));
                    issuedActions.AddRange(await GenerateIssuedActionsFromSystemAction(trigger, userAction, systemAction, projectID));
                }
            }

            i++;
            if (i == triggers.Count)
            {
                break;
            }
        }

        return issuedActions;
    }

    public static List<EntityIdPair> ConstructEntityIdPairs(List<IssuedAction> issuedActions)
    {
        List<EntityIdPair> pairs = [];

        foreach (var issuedAction in issuedActions)
        {
            pairs.Add(new(issuedAction.TargetID, issuedAction.TargetType));
        }

        return pairs;
    }

    public static async Task<bool> ValidateIssuedActionSet(List<IssuedAction> issuedActions, Dictionary<EntityIdPair, int> entityVersions)
    {
        List<Project> projects;
        List<TaskItem> tasks;
        List<Tag> tags;

        using (var context = AppDbContextFactory.CreateContext())
        {
            projects = await context.Projects.ToListAsync();
            tasks = await context.Tasks.ToListAsync();
            tags = await context.Tags.ToListAsync();
        }

        foreach (var issuedAction in issuedActions)
        {
            EntityIdPair pair = new(issuedAction.TargetID, issuedAction.TargetType);
            
            switch (pair.EntityType)
            {
                case DomainEntityType.Project:
                    var project = projects.Single(p => p.ID == pair.ID);

                    if (project.Version != entityVersions[pair])
                    {
                        return false;
                    }

                    break;
                
                case DomainEntityType.Task:
                    var task = tasks.Single(p => p.ID == pair.ID);

                    if (task.Version != entityVersions[pair])
                    {
                        return false;
                    }

                    break;
                
                case DomainEntityType.Tag:
                    var tag = tags.Single(p => p.ID == pair.ID);

                    if (tag.Version != entityVersions[pair])
                    {
                        return false;
                    }

                    break;
            }
        }
        
        return true;
    }

    public static async Task PersistPendingActions(List<IssuedAction> issuedActions)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            context.IssuedActions.AddRange(issuedActions);
            await context.SaveChangesAsync();
        }
    }

    public static async Task ExecutePendingIssuedActions()
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var issuedActions = await context.IssuedActions
                .Where(ia => ia.Status == IssuedActionStatus.Pending)
                .ToListAsync();
            
            foreach (var issuedAction in issuedActions)
            {
                switch (issuedAction.ActionType)
                {
                    case ActionType.LogActivity:
                        // TODO: implement after LoggingService is done
                        break;
                    
                    case ActionType.SendNotification:
                        // TODO: implement after NotificationService is done
                        break;

                    case ActionType.CreateEntity:
                        switch (issuedAction.TargetType)
                        {
                            case DomainEntityType.Task:
                                if (issuedAction.Change is null
                                 || issuedAction.RelatedEntityType is null
                                 || issuedAction.RelatedEntityID is null)
                                {
                                    throw new NullReferenceException();
                                }

                                await DomainService.CreateTask(context, issuedAction.RelatedEntityID.Value, issuedAction.Change);
                                break;
                            
                            case DomainEntityType.Tag:
                                if (issuedAction.Change is null
                                 || issuedAction.RelatedEntityType is null
                                 || issuedAction.RelatedEntityID is null)
                                {
                                    throw new NullReferenceException();
                                }

                                await DomainService.CreateTag(context, issuedAction.RelatedEntityID.Value, issuedAction.Change);
                                break;
                            
                            default:
                                throw new UnexpectedEntityTypeException();
                        }

                        break;

                    case ActionType.DeleteEntity:
                        switch (issuedAction.TargetType)
                        {
                            case DomainEntityType.Project:
                                await DomainService.DeleteProject(context, issuedAction.TargetID);
                                break;

                            case DomainEntityType.Task:
                                await DomainService.DeleteTask(context, issuedAction.TargetID);
                                break;
                            
                            case DomainEntityType.Tag:
                                await DomainService.DeleteTag(context, issuedAction.TargetID);
                                break;
                            
                            default:
                                throw new UnexpectedEntityTypeException();
                        }

                        break;
                    
                    case ActionType.ChangeAttribute:
                        if (issuedAction.Change is null)
                        {
                            throw new NullReferenceException();
                        }
                        
                        await DomainService.ApplyChange(context, issuedAction.Change);
                        break;
                    
                    case ActionType.ChangeUserRole:
                        if (issuedAction.Change is null || issuedAction.Change.Role is null || issuedAction.RelatedEntityID is null)
                        {
                            throw new NullReferenceException();
                        }

                        await DomainService.ChangeUserRole(context, issuedAction.RelatedEntityID.Value, issuedAction.TargetID, issuedAction.Change.Role.Value);
                        break;
                    
                    case ActionType.AssignUser:
                        switch (issuedAction.RelatedEntityType)
                        {
                            case DomainEntityType.Project:
                                if (issuedAction.Change is null || issuedAction.Change.Role is null || issuedAction.RelatedEntityID is null)
                                {
                                    throw new NullReferenceException();
                                }

                                await DomainService.AssignUserToProject(context, issuedAction.TargetID, issuedAction.RelatedEntityID.Value, issuedAction.Change.Role.Value);
                                break;
                            
                            case DomainEntityType.Task:
                                if (issuedAction.RelatedEntityID is null)
                                {
                                    throw new NullReferenceException();
                                }

                                await DomainService.AssignUserToTask(context, issuedAction.TargetID, issuedAction.RelatedEntityID.Value);
                                break;
                            
                            default:
                                throw new UnexpectedEntityTypeException();
                        }

                        break;
                        
                    case ActionType.UnassignUser:
                        switch (issuedAction.RelatedEntityType)
                        {
                            case DomainEntityType.Project:
                                if (issuedAction.RelatedEntityID is null)
                                {
                                    throw new NullReferenceException();
                                }

                                await DomainService.UnassignUserFromProject(context, issuedAction.TargetID, issuedAction.RelatedEntityID.Value);
                                break;
                            
                            case DomainEntityType.Task:
                                if (issuedAction.RelatedEntityID is null)
                                {
                                    throw new NullReferenceException();
                                }

                                await DomainService.UnassignUserFromTask(context, issuedAction.TargetID, issuedAction.RelatedEntityID.Value);
                                break;
                            
                            default:
                                throw new UnexpectedEntityTypeException();
                        }

                        break;
                
                    default:
                        throw new UnexpectedActionTypeException();
                }
            }

            await context.SaveChangesAsync();
        }
    }
}

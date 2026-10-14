namespace Workflow.Enums;

public enum State
{
    None,
    UserStatusActive,
    UserStatusInactive,
    TaskStatusTodo,
    TaskStatusInProgress,
    TaskStatusOnHold,
    TaskStatusBlocked,
    TaskStatusCompleted,
    TaskPriorityLow,
    TaskPriorityMedium,
    TaskPriorityHigh,
    TaskPriorityUrgent,
    NoUsersAssignedToInProgressTask,
    UsersAssignedToCompletedTask,
    ProjectStatusActive,
    ProjectStatusOnHold,
    ProjectStatusArchived,
    ProjectStatusCompleted,
    NoTasksInProgress,
    TasksInProgress
}

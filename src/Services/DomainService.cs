using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database;
using Infrastructure.Entities;
using Infrastructure.Enums;
using Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Services;

public static class DomainService
{
    public static async Task<bool> IsUserAssignedToProject(AppDbContext context, int userID, int projectID)
    {
        return await context.ProjectAssignments
            .AnyAsync(pa => pa.UserID == userID && pa.ProjectID == projectID);
    }

    public static async Task<bool> IsProjectDuplicate(AppDbContext context, int ownerID, string projectName, int? projectID = null)
    {

        return await context.Projects
            .AnyAsync(p => p.ID != projectID && p.OwnerID == ownerID && p.Name == projectName);
    }

    public static async Task<bool> IsTaskDuplicate(AppDbContext context, int projectID, string taskName, int? taskID = null)
    {
        return await context.Tasks
            .AnyAsync(t => t.ID != taskID && t.ProjectID == projectID && t.Name == taskName);
    }

    public static async Task<bool> IsTagDuplicate(AppDbContext context, int projectID, string tagName, int? tagID = null)
    {
        return await context.Tags
            .AnyAsync(t => t.ID != tagID && t.ProjectID == projectID && t.Name == tagName);
    }

    public static async Task<Project> GetProjectByID(AppDbContext context, int projectID)
    {
        var project = await context.Projects
            .Where(p => p.ID == projectID)
            .SingleOrDefaultAsync();
        
        if (project is null)
        {
            throw new InexistentProjectException();
        }

        return project;
    }

    public static async Task<TaskItem> GetTaskByID(AppDbContext context, int taskID)
    {
        var task = await context.Tasks
            .Where(t => t.ID == taskID)
            .SingleOrDefaultAsync();
        
        if (task is null)
        {
            throw new InexistentTaskException();
        }

        return task;
    }

    public static async Task<Tag> GetTagByID(AppDbContext context, int tagID)
    { 
        var tag = await context.Tags
            .Where(t => t.ID == tagID)
            .SingleOrDefaultAsync();
        
        if (tag is null)
        {
            throw new InexistentTagException();
        }

        return tag;
    }

    public static async Task<List<User>> GetProjectMembers(AppDbContext context, int projectID)
    {
        return await context.ProjectAssignments
            .Where(pa => pa.ProjectID == projectID)
            .Join(
                context.Users,
                pa => pa.UserID,
                u => u.ID,
                (pa, u) => u)
            .ToListAsync();
    }

    public static async Task<ProjectRole> GetUserRoleInProject(AppDbContext context, int userID, int projectID)
    {
        var assignment = await context.ProjectAssignments
            .SingleOrDefaultAsync(pa => pa.UserID == userID && pa.ProjectID == projectID);
        
        if (assignment is null)
        {
            throw new UserNotAssignedToProjectException();
        }

        return assignment.Role;
    }

    public static async Task<List<User>> GetUsersAssignedToTask(AppDbContext context, int taskID)
    {
        return await context.TaskAssignments
            .Where(ta => ta.TaskID == taskID)
            .Join(
                context.Users,
                ta => ta.UserID,
                u => u.ID,
                (ta, u) => u)
            .ToListAsync();
    }

    public static async Task<List<Project>> GetProjectsByUser(AppDbContext context, int userID)
    {
        return await context.ProjectAssignments
            .Where(pa => pa.UserID == userID)
            .Join(
                context.Projects,
                pa => pa.ProjectID,
                p => p.ID,
                (pa, p) => p)
            .ToListAsync();
    }

    public static async Task<List<TaskItem>> GetTasksByProject(AppDbContext context, int projectID)
    {
        return await context.Tasks
            .Where(t => t.ProjectID == projectID)
            .ToListAsync();
    }

    public static async Task<List<TaskItem>> GetTasksByTag(AppDbContext context, int tagID)
    {
        return await context.TaskTagLinks
            .Where(l => l.TagID == tagID)
            .Join(
                context.Tasks,
                l => l.TaskID,
                t => t.ID,
                (l, t) => t)
            .ToListAsync();
    }

    public static async Task<List<TaskItem>> GetTasksByStatus(AppDbContext context, int projectID, TaskItemStatus taskStatus)
    {
        return await context.Tasks
            .Where(t => t.ProjectID == projectID && t.Status == taskStatus)
            .ToListAsync();
    }

    public static async Task<List<TaskItem>> GetTasksByPriority(AppDbContext context, int projectID, TaskItemPriority taskPriority)
    {

        return await context.Tasks
            .Where(t => t.ProjectID == projectID && t.Priority == taskPriority)
            .ToListAsync();
    }

    public static async Task<List<TaskItem>> GetTasksByName(AppDbContext context, int projectID, string taskName)
    {

        return await context.Tasks
            .Where(t => t.ProjectID == projectID && t.Name.Contains(taskName))
            .ToListAsync();
    }

    public static async Task<List<TaskItem>> GetTasksAssignedToUser(AppDbContext context, int projectID, int userID)
    {
        return await context.TaskAssignments
            .Where(ta => ta.UserID == userID)
            .Join(
                context.Tasks,
                l => l.TaskID,
                t => t.ID,
                (l, t) => t)
            .Where(t => t.ProjectID == projectID)
            .ToListAsync();
    }

    public static async Task<List<Tag>> GetTagsByProject(AppDbContext context, int projectID)
    {
        return await context.Tags
            .Where(t => t.ProjectID == projectID)
            .ToListAsync();
    }

    public static async Task<List<Tag>> GetTagsByTask(AppDbContext context, int taskID)
    {
        return await context.TaskTagLinks
            .Where(l => l.TaskID == taskID)
            .Join(
                context.Tags,
                l => l.TagID,
                t => t.ID,
                (l, t) => t)
            .ToListAsync();
    }

    public static async Task<List<Tag>> GetTagsByName(AppDbContext context, int projectID, string tagName)
    {
        return await context.Tags
            .Where(t => t.ProjectID == projectID && t.Name.Contains(tagName))
            .ToListAsync();
    }

    public static async Task<int> CreateProject(AppDbContext context, int ownerID, string projectName, string projectDescription, ProjectStatus projectStatus, List<TaskItem> tasks, List<Tag> tags, List<TaskTagLink> taskTagLinks)
    {
        bool isDuplicate = await context.Projects
            .AnyAsync(p => p.OwnerID == ownerID && p.Name == projectName);
        
        if (isDuplicate)
        {
            throw new DuplicateProjectException();
        }

        Project project = new(ownerID, projectName, projectDescription, projectStatus, null, null, null);

        if (project.ID == 0)
        {
            throw new NullReferenceException();
        }

        await context.SaveChangesAsync();

        List<TaskItem> newTasks = [];
        Dictionary<int, TaskItem> newTasksByOldID = [];

        foreach (var t in tasks)
        {
            TaskItem newTask = new(project.ID, t.Name, t.Description, t.Status, t.Priority, t.DueDate, null, null);

            if (t.Status == TaskItemStatus.Completed)
            {
                newTask.Complete();
            }

            newTasks.Add(newTask);
            newTasksByOldID.Add(t.ID, newTask);
        }

        List<Tag> newTags = [];
        Dictionary<int, Tag> newTagsByOldID = [];

        foreach (var t in tags)
        {

            Tag newTag = new(project.ID, t.Name);
            newTags.Add(newTag);
            newTagsByOldID.Add(t.ID, newTag);
        }

        context.Tasks.AddRange(newTasks);
        context.Tags.AddRange(newTags);
        await context.SaveChangesAsync();

        List<TaskTagLink> newLinks = [];
        
        foreach (var link in taskTagLinks)
        {
            int newTaskID = newTasksByOldID[link.TaskID].ID;
            int newTagID = newTagsByOldID[link.TagID].ID;

            newLinks.Add(new(newTaskID, newTagID));
        }

        context.TaskTagLinks.AddRange(newLinks);
        await context.SaveChangesAsync();

        await WorkflowEngine.CreateSystemActionPreferences(project.ID);
        return project.ID;
    }

    public static async Task CreateTask(AppDbContext context, int projectID, Change change)
    {
        if (change.Name is null || change.Description is null || change.Status is null || change.Priority is null || change.DueDate is null)
        {
            throw new NullReferenceException();
        }

        bool isDuplicate = await context.Tasks
            .AnyAsync(t => t.ProjectID == projectID && t.Name == change.Name);
        
        if (isDuplicate)
        {
            throw new DuplicateTaskException();
        }

        var status = change.Status switch
        {
            DomainEntityStatus.Todo => TaskItemStatus.Todo,
            DomainEntityStatus.InProgress => TaskItemStatus.InProgress,
            DomainEntityStatus.OnHold => TaskItemStatus.OnHold,
            DomainEntityStatus.Blocked => TaskItemStatus.Blocked,
            DomainEntityStatus.Completed => TaskItemStatus.Completed,
            _ => TaskItemStatus.Todo
        };

        TaskItem newTask = new(projectID, change.Name, change.Description, status, change.Priority.Value, change.DueDate, null, null);
        context.Tasks.Add(newTask);
    }

    public static async Task CreateTag(AppDbContext context, int projectID, Change change)
    {
        if (change.Name is null)
        {
            throw new NullReferenceException();
        }
        
        bool isDuplicate = await context.Tags
            .AnyAsync(t => t.ProjectID == projectID && t.Name == change.Name);
        
        if (isDuplicate)
        {
            throw new DuplicateTagException();
        }

        Tag newTag = new(projectID, change.Name);
        context.Tags.Add(newTag);
    }

    public static async Task AssignTagToTask(AppDbContext context, int taskID, int tagID)
    {
        bool isDuplicate = await context.TaskTagLinks
            .AnyAsync(l => l.TaskID == taskID && l.TagID == tagID);

        if (!isDuplicate)
        {
            TaskTagLink newLink = new(taskID, tagID);
            context.TaskTagLinks.Add(newLink);
        }
    }

    public static async Task RemoveTagFromTask(AppDbContext context, int taskID, int tagID)
    {
        bool linkExists = await context.TaskTagLinks
            .AnyAsync(l => l.TaskID == taskID && l.TagID == tagID);
        
        if (linkExists)
        {
            TaskTagLink link = new(taskID, tagID);
            context.TaskTagLinks.Remove(link);
        }
    }

    public static async Task ChangeUserRole(AppDbContext context, int projectID, int userID, ProjectRole newRole)
    {
        var projectAssignment = await context.ProjectAssignments
            .SingleOrDefaultAsync(pa => pa.UserID == userID && pa.ProjectID == projectID);

        if (projectAssignment is null)
        {
            throw new NullReferenceException();
        }

        projectAssignment.Role = newRole;
    }

    public static async Task ApplyChange(AppDbContext context, Change localChange)
    {
        switch (localChange.TargetType)
        {
            case DomainEntityType.Project:
                var project = await context.Projects
                    .SingleOrDefaultAsync(f => f.ID == localChange.TargetID);
                
                if (project is null)
                {
                    throw new InexistentProjectException();
                }

                if (localChange.Name is not null)
                {
                    project.Name = localChange.Name;
                }

                if (localChange.Description is not null)
                {
                    project.Description = localChange.Description;
                }

                if (localChange.Status is not null)
                {
                    switch (localChange.Status)
                    {
                        case DomainEntityStatus.Active:
                            project.ChangeStatus(ProjectStatus.Active);
                            break;
                        
                        case DomainEntityStatus.Archived:
                            project.ChangeStatus(ProjectStatus.Archived);
                            break;
                        
                        case DomainEntityStatus.OnHold:
                            project.ChangeStatus(ProjectStatus.OnHold);
                            break;
                        
                        case DomainEntityStatus.Completed:
                            project.Complete();
                            break;
                    }
                }

                if (localChange.Name is not null
                    || localChange.Description is not null
                    || localChange.Status is not null)
                {
                    project.IncreaseVersion();                        
                }

                break;
            
            case DomainEntityType.Task:
                var task = await context.Tasks
                    .SingleOrDefaultAsync(f => f.ID == localChange.TargetID);
                
                if (task is null)
                {
                    throw new InexistentTaskException();
                }

                if (localChange.Name is not null)
                {
                    task.Name = localChange.Name;
                }

                if (localChange.Description is not null)
                {
                    task.Description = localChange.Description;
                }

                if (localChange.Status is not null)
                {
                    switch (localChange.Status)
                    {
                        case DomainEntityStatus.InProgress:
                            task.ChangeStatus(TaskItemStatus.InProgress);
                            break;
                        
                        case DomainEntityStatus.Todo:
                            task.ChangeStatus(TaskItemStatus.Todo);
                            break;
                        
                        case DomainEntityStatus.OnHold:
                            task.ChangeStatus(TaskItemStatus.OnHold);
                            break;
                        
                        case DomainEntityStatus.Blocked:
                            task.ChangeStatus(TaskItemStatus.Blocked);
                            break;
                        
                        case DomainEntityStatus.Completed:
                            task.Complete();
                            break;
                    }
                }

                if (localChange.Priority is not null)
                {
                    switch (localChange.Priority)
                    {
                        case TaskItemPriority.Low:
                            task.Priority = TaskItemPriority.Low;
                            break;

                        case TaskItemPriority.Medium:
                            task.Priority = TaskItemPriority.Medium;
                            break;
                        
                        case TaskItemPriority.High:
                            task.Priority = TaskItemPriority.High;
                            break;
                        
                        case TaskItemPriority.Urgent:
                            task.Priority = TaskItemPriority.Urgent;
                            break;
                    }
                }

                if (localChange.DueDateChanged)
                {
                    task.ChangeDueDate(localChange.DueDate);
                }

                if (localChange.Name is not null
                    || localChange.Description is not null
                    || localChange.Status is not null
                    || localChange.Priority is not null
                    || localChange.DueDateChanged)
                {
                    task.IncreaseVersion();                        
                }

                break;
            
            case DomainEntityType.Tag:
                var tag = await context.Tags
                    .SingleOrDefaultAsync(f => f.ID == localChange.TargetID);
                
                if (tag is null)
                {
                    throw new InexistentTagException();
                }

                if (localChange.Name is not null)
                {
                    tag.Name = localChange.Name;
                    tag.IncreaseVersion();                        
                }

                break;
        }
    }

    public static async Task DeleteProject(AppDbContext context, int projectID)
    {
        bool projectExists = await context.Projects
            .AnyAsync(f => f.ID == projectID);
        
        if (!projectExists)
        {
            return;
        }

        Project? project = await context.Projects
            .Where(f => f.ID == projectID)
            .SingleAsync();
        
        context.Projects.Remove(project);
    }

    public static async Task DeleteTask(AppDbContext context, int taskID)
    {
        bool taskExists = await context.Tasks
            .AnyAsync(f => f.ID == taskID);
        
        if (!taskExists)
        {
            return;
        }

        TaskItem? task = await context.Tasks
            .Where(f => f.ID == taskID)
            .SingleAsync();
        
        context.Tasks.Remove(task);
    }

    public static async Task DeleteTag(AppDbContext context, int tagID)
    {
        bool tagExists = await context.Tags
            .AnyAsync(f => f.ID == tagID);
        
        if (!tagExists)
        {
            return;
        }

        Tag? tag = await context.Tags
            .Where(f => f.ID == tagID)
            .SingleAsync();
        
        context.Tags.Remove(tag);
    }

    public static async Task AssignUserToProject(AppDbContext context, int userID, int projectID, ProjectRole role)
    {
        bool alreadyAssigned = await context.ProjectAssignments
            .AnyAsync(pa => pa.UserID == userID && pa.ProjectID == projectID);
        
        if (alreadyAssigned)
        {
            return;
        }

        context.ProjectAssignments.Add(new(userID, projectID, role));
    }

    public static async Task AssignUserToTask(AppDbContext context, int userID, int taskID)
    {
        var task = await context.Tasks.SingleAsync(t => t.ID == taskID);

        bool assignedToProject = await context.ProjectAssignments
            .AnyAsync(pa => pa.UserID == userID && pa.ProjectID == task.ProjectID);
        
        if (!assignedToProject)
        {
            throw new UserNotAssignedToProjectException();
        }
        
        bool alreadyAssigned = await context.TaskAssignments
            .AnyAsync(pa => pa.UserID == userID && pa.TaskID == taskID);
        
        if (alreadyAssigned)
        {
            return;
        }

        context.TaskAssignments.Add(new(userID, taskID));
    }

    public static async Task UnassignUserFromProject(AppDbContext context, int userID, int projectID)
    {
        var assignment = await context.ProjectAssignments
            .SingleOrDefaultAsync(pa => pa.UserID == userID && pa.ProjectID == projectID);
        
        if (assignment is null)
        {
            return;
        }

        context.ProjectAssignments.Remove(assignment);
    }

    public static async Task UnassignUserFromTask(AppDbContext context, int userID, int taskID)
    {
        var assignment = await context.TaskAssignments
            .SingleOrDefaultAsync(pa => pa.UserID == userID && pa.TaskID == taskID);
        
        if (assignment is null)
        {
            return;
        }

        context.TaskAssignments.Remove(assignment);
    }
}

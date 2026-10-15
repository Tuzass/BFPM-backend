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
    public static async Task<bool> IsUserAssignedToProject(int userID, int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.ProjectAssignments
                .AnyAsync(pa => pa.UserID == userID && pa.ProjectID == projectID);
        }
    }

    public static async Task<bool> IsProjectDuplicate(int ownerID, string projectName, int? projectID = null)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Projects
                .AnyAsync(p => p.ID != projectID && p.OwnerID == ownerID && p.Name == projectName);
        }
    }

    public static async Task<bool> IsTaskDuplicate(int projectID, string taskName, int? taskID = null)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tasks
                .AnyAsync(t => t.ID != taskID && t.ProjectID == projectID && t.Name == taskName);
        }
    }

    public static async Task<bool> IsTagDuplicate(int projectID, string tagName, int? tagID = null)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tags
                .AnyAsync(t => t.ID != tagID && t.ProjectID == projectID && t.Name == tagName);
        }
    }

    public static async Task<Project> GetProjectByID(int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
    }

    public static async Task<TaskItem> GetTaskByID(int taskID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
    }

    public static async Task<Tag> GetTagByID(int tagID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
    }

    public static async Task<List<User>> GetProjectMembers(int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
    }

    public static async Task<ProjectRole> GetUserRoleInProject(int userID, int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var assignment = await context.ProjectAssignments
                .SingleOrDefaultAsync(pa => pa.UserID == userID && pa.ProjectID == projectID);
            
            if (assignment is null)
            {
                throw new UserNotAssignedToProjectException();
            }

            return assignment.Role;
        }
    }

    public static async Task<List<User>> GetUsersAssignedToTask(int taskID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
    }

    public static async Task<List<Project>> GetProjectsByUser(int userID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
    }

    public static async Task<List<TaskItem>> GetTasksByProject(int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tasks
                .Where(t => t.ProjectID == projectID)
                .ToListAsync();
        }
    }

    public static async Task<List<TaskItem>> GetTasksByTag(int tagID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
    }

    public static async Task<List<TaskItem>> GetTasksByStatus(int projectID, TaskItemStatus taskStatus)
    {
        var project = await GetProjectByID(projectID);
        
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tasks
                .Where(t => t.ProjectID == projectID && t.Status == taskStatus)
                .ToListAsync();
        }
    }

    public static async Task<List<TaskItem>> GetTasksByPriority(int projectID, TaskItemPriority taskPriority)
    {
        var project = await GetProjectByID(projectID);
        
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tasks
                .Where(t => t.ProjectID == projectID && t.Priority == taskPriority)
                .ToListAsync();
        }
    }

    public static async Task<List<TaskItem>> GetTasksByName(int projectID, string taskName)
    {
        var project = await GetProjectByID(projectID);
        
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tasks
                .Where(t => t.ProjectID == projectID && t.Name.Contains(taskName))
                .ToListAsync();
        }
    }

    public static async Task<List<TaskItem>> GetTasksAssignedToUser(int projectID, int userID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
    }

    public static async Task<List<Tag>> GetTagsByProject(int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tags
                .Where(t => t.ProjectID == projectID)
                .ToListAsync();
        }
    }

    public static async Task<List<Tag>> GetTagsByTask(int taskID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
    }

    public static async Task<List<Tag>> GetTagsByName(int projectID, string tagName)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Tags
                .Where(t => t.ProjectID == projectID && t.Name.Contains(tagName))
                .ToListAsync();
        }
    }

    public static async Task<int> CreateProject(int ownerID, string projectName, string projectDescription, ProjectStatus projectStatus, List<TaskItem> tasks, List<Tag> tags, List<TaskTagLink> taskTagLinks)
    {
        using (var context = AppDbContextFactory.CreateContext())
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

            return project.ID;
        }
    }

    public static async Task CreateTask(int projectID, string taskName, string taskDescription, TaskItemStatus taskStatus, TaskItemPriority taskPriority, DateTime? taskDueDate)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            bool isDuplicate = await context.Tasks
                .AnyAsync(t => t.ProjectID == projectID && t.Name == taskName);
            
            if (isDuplicate)
            {
                throw new DuplicateTaskException();
            }

            TaskItem newTask = new(projectID, taskName, taskDescription, taskStatus, taskPriority, taskDueDate, null, null);
            context.Tasks.Add(newTask);
            await context.SaveChangesAsync();
        }
    }

    public static async Task CreateTag(int projectID, string tagName)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            bool isDuplicate = await context.Tags
                .AnyAsync(t => t.ProjectID == projectID && t.Name == tagName);
            
            if (isDuplicate)
            {
                throw new DuplicateTagException();
            }

            Tag newTag = new(projectID, tagName);
            context.Tags.Add(newTag);
            await context.SaveChangesAsync();
        }
    }

    public static async Task AssignTagToTask(int taskID, int tagID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            bool isDuplicate = await context.TaskTagLinks
                .AnyAsync(l => l.TaskID == taskID && l.TagID == tagID);

            if (!isDuplicate)
            {
                TaskTagLink newLink = new(taskID, tagID);
                context.TaskTagLinks.Add(newLink);
                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task RemoveTagFromTask(int taskID, int tagID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            bool linkExists = await context.TaskTagLinks
                .AnyAsync(l => l.TaskID == taskID && l.TagID == tagID);
            
            if (linkExists)
            {
                TaskTagLink link = new(taskID, tagID);
                context.TaskTagLinks.Remove(link);
                await context.SaveChangesAsync();
            }
        }
    }

    public static async Task ApplyLocalChange(LocalChange localChange)
    {
        using (var context = AppDbContextFactory.CreateContext())
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

            await context.SaveChangesAsync();
        }
    }

    public static async Task DeleteProject(int projectID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
            await context.SaveChangesAsync();
        }
    }

    public static async Task DeleteTask(int taskID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
            await context.SaveChangesAsync();
        }
    }

    public static async Task DeleteTag(int tagID)
    {
        using (var context = AppDbContextFactory.CreateContext())
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
            await context.SaveChangesAsync();
        }
    }
}

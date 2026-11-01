using System.Text.Json.Serialization;
using Domain.Enums;
using Infrastructure.Enums;

namespace Infrastructure.Entities;

public class Change
{
    public DomainEntityType TargetType {get; private set;}
    public int TargetID {get; private set;}
    public string? Name {get; set;}
    public string? Description {get; set;}
    public DomainEntityStatus? Status {get; set;}
    public TaskItemPriority? Priority {get; set;}
    public DateTime? DueDate {get; set;}
    public bool DueDateChanged {get; set;}
    public ProjectRole? Role {get; set;}

    private Change()
    {
        
    }

    [JsonConstructor]
    public Change(DomainEntityType targetType, int targetID, ProjectRole? role, string? name, string? description = null, DomainEntityStatus? status = null, TaskItemPriority? priority = null, DateTime? dueDate = null, bool dueDateChanged = false)
    {
        TargetType = targetType;
        TargetID = targetID;
        Role = role;
        Name = name;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        DueDateChanged = dueDateChanged;
    }
}

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
    public Change(DomainEntityType targetType, int targetID, string? name, string? description, DomainEntityStatus? status, TaskItemPriority? priority, DateTime? dueDate, bool dueDateChanged, ProjectRole? role)
    {
        TargetType = targetType;
        TargetID = targetID;
        Name = name;
        Description = description;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        DueDateChanged = dueDateChanged;
        Role = role;
    }
}

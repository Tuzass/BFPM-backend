using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace API.Requests;

public class UpdateTaskRequest
{
    [StringLength(50, MinimumLength = 3)]
    public string? Name {get; set;} = string.Empty;

    [StringLength(2000)]
    public string? Description {get; set;} = string.Empty;

    public TaskItemStatus? Status {get; set;}

    public TaskItemPriority? Priority {get; set;}

    public DateTime? DueDate {get; set;}

    [Required]
    public bool DueDateChanged {get; set;}
}

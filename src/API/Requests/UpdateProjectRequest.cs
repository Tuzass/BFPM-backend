using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace API.Requests;

public class UpdateProjectRequest
{
    [StringLength(50, MinimumLength = 3)]
    public string? Name {get; set;} = string.Empty;

    [StringLength(2000)]
    public string? Description {get; set;} = string.Empty;

    public ProjectStatus? Status {get; set;}
}

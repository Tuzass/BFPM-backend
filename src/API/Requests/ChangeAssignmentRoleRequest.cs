using System.ComponentModel.DataAnnotations;
using Infrastructure.Enums;

namespace API.Requests;

public class ChangeAssignmentRoleRequest
{
    [Required]
    public int UserID {get; set;}

    [Required]
    public int ProjectID {get; set;}

    [Required]
    public ProjectRole Role {get; set;}
}

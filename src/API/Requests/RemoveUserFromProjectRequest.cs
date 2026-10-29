using System.ComponentModel.DataAnnotations;

namespace API.Requests;

public class RemoveUserFromProjectRequest
{
    [Required]
    public int UserID {get; set;}

    [Required]
    public int ProjectID {get; set;}
}

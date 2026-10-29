using System.ComponentModel.DataAnnotations;

namespace API.Requests;

public class RemoveUserFromTaskRequest
{
    [Required]
    public int UserID {get; set;}

    [Required]
    public int TaskID {get; set;}
}

using System.ComponentModel.DataAnnotations;

namespace API.Requests;

public class DeleteTaskRequest
{
    [Required]
    public int ID {get; set;}
}

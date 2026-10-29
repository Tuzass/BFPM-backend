using System.ComponentModel.DataAnnotations;

namespace API.Requests;

public class DeleteProjectRequest
{
    [Required]
    public int ID {get; set;}
}

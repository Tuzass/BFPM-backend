using System.ComponentModel.DataAnnotations;

namespace API.Requests;

public class DeleteTagRequest
{
    [Required]
    public int ID {get; set;}
}

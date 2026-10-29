using System.ComponentModel.DataAnnotations;

namespace API.Requests;

public class UpdateTagRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name {get; set;} = string.Empty;
}

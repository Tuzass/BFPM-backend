namespace API.Responses;

public class RegisterConflictResponse
{
    public string Message {get; set;} = null!;
    public string Field {get; set;} = null!;

    public RegisterConflictResponse(string message, string field)
    {
        Message = message;
        Field = field;
    }

    private RegisterConflictResponse()
    {
        
    }
}

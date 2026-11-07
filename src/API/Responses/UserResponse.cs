namespace API.Responses;

public class UserResponse
{
    public int ID {get; set;}
    public string UserName {get; set;} = null!;
    public string Email {get; set;} = null!;

    public UserResponse(int id, string userName, string email)
    {
        ID = id;
        UserName = userName;
        Email = email;
    }

    private UserResponse()
    {
        
    }
}

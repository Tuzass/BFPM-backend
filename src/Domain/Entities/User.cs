namespace Domain.Entities;

public class User
{
    public int ID {get; private set;}
    public string Username {get; set;} = null!;
    public string Email {get; set;} = null!;
    public string PasswordHash {get; set;} = null!;
    public bool IsActive {get; set;}

    private User() { }

    public User(string username, string email, string passwordHash)
    {
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;
    }
}

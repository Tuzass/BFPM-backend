namespace Domain.Entities;

public class User
{
    public int ID {get; private set;}
    public string Username {get; private set;} = null!;
    public string Email {get; private set;} = null!;
    public string PasswordHash {get; private set;} = null!;
    public bool IsActive {get; set;}

    private User() { }

    public User(int id, string username, string email, string passwordHash)
    {
        ID = id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        IsActive = true;
    }
}

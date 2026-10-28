using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User: IdentityUser<int>
{
    public bool IsActive {get; set;}

    private User() { }

    public User(string username, string email)
    {
        UserName = username;
        Email = email;
        IsActive = true;
    }
}

using Domain.Entities;
using Services.Exceptions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Services;

public static class UserService
{
    public static async Task CreateUser(AppDbContext context, string username, string email, string passwordHash)
    {
        bool isUsernameDuplicate = await context.Users
            .AnyAsync(u => u.Username == username);
        
        if (isUsernameDuplicate)
        {
            throw new DuplicateUsernameException();
        }

        User user = new(username, email, passwordHash);
        context.Add(user);

    // TODO: create notification
    // TODO: log activity?
    }

    public static void Login()
    {
        // TODO
    }

    public static async Task ChangeUsername(AppDbContext context,int userID, string newUsername)
    {
        var user = await context.Users
            .SingleOrDefaultAsync(u => u.ID == userID);
        
        if (user is null)
        {
            throw new InexistentUserException();
        }
        
        user.Username = newUsername;
    }

    public static async Task ChangeEmail(AppDbContext context, int userID, string newEmail)
    {
        var user = await context.Users
            .SingleOrDefaultAsync(u => u.ID == userID);
        
        if (user is null)
        {
            throw new InexistentUserException();
        }
        
        user.Email = newEmail;
    }

    public static async Task ChangePassword(AppDbContext context, int userID, string newPasswordHash)
    {
        var user = await context.Users
            .SingleOrDefaultAsync(u => u.ID == userID);
        
        if (user is null)
        {
            throw new InexistentUserException();
        }
        
        user.PasswordHash = newPasswordHash;
    }

    public static async Task<User> GetUser(AppDbContext context, int userID)
    {
        var user = await context.Users
            .SingleOrDefaultAsync(u => u.ID == userID);
        
        if (user is null || !user.IsActive)
        {
            throw new InexistentUserException();
        }

        return user;
    }

    public static async Task ActivateUser(AppDbContext context, int userID)
    {
        var user = await context.Users
            .SingleOrDefaultAsync(u => u.ID == userID);
        
        if (user is null)
        {
            throw new InexistentUserException();
        }
        
        user.IsActive = true;
    }

    public static async Task DeactivateUser(AppDbContext context, int userID)
    {
        var user = await context.Users
            .SingleOrDefaultAsync(u => u.ID == userID);
        
        if (user is null)
        {
            throw new InexistentUserException();
        }
        
        user.IsActive = false;
    }
}

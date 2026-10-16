using Domain.Entities;
using Services.Exceptions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Services;

public static class UserService
{
    public static async Task CreateUser(string username, string email, string passwordHash)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            bool isUsernameDuplicate = await context.Users
                .AnyAsync(u => u.Username == username);
            
            if (isUsernameDuplicate)
            {
                throw new DuplicateUsernameException();
            }

            User user = new(username, email, passwordHash);
            context.Add(user);
            await context.SaveChangesAsync();
        }

        // TODO: create notification
        // TODO: log activity?
    }

    public static void Login()
    {
        // TODO
    }

    public static async Task ChangeUsername(int userID, string newUsername)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var user = await context.Users
                .SingleOrDefaultAsync(u => u.ID == userID);
            
            if (user is null)
            {
                throw new InexistentUserException();
            }
            
            user.Username = newUsername;
            await context.SaveChangesAsync();
        }
    }

    public static async Task ChangeEmail(int userID, string newEmail)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var user = await context.Users
                .SingleOrDefaultAsync(u => u.ID == userID);
            
            if (user is null)
            {
                throw new InexistentUserException();
            }
            
            user.Email = newEmail;
            await context.SaveChangesAsync();
        }
    }

    public static async Task ChangePassword(int userID, string newPasswordHash)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var user = await context.Users
                .SingleOrDefaultAsync(u => u.ID == userID);
            
            if (user is null)
            {
                throw new InexistentUserException();
            }
            
            user.PasswordHash = newPasswordHash;
            await context.SaveChangesAsync();
        }
    }

    public static async Task<User> GetUser(int userID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var user = await context.Users
                .SingleOrDefaultAsync(u => u.ID == userID);
            
            if (user is null || !user.IsActive)
            {
                throw new InexistentUserException();
            }

            return user;
        }
    }

    public static async Task DeleteUser(int userID)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            var user = await GetUser(userID);
            
            context.Remove(user);
            await context.SaveChangesAsync();
        }
    }

    public static async Task<List<User>> GetUsers()
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            return await context.Users
                .ToListAsync();
        }
    }
}

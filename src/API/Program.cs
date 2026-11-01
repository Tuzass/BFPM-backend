using Domain.Entities;
using Infrastructure.Database;
using Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace API;

sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("AppDatabase");

        if (connectionString is null)
        {
            throw new NullReferenceException();
        }

        AppDbContextFactory.ConnectionString = connectionString;

        using (var context = AppDbContextFactory.CreateContext())
        {
            context.Database.ExecuteSqlRaw("CREATE EXTENSION IF NOT EXISTS citext;");
        }

        builder.Services.AddDbContext<AppDbContext>(
            options => options.UseNpgsql(builder.Configuration.GetConnectionString("AppDatabase")));

        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        builder.Services.AddIdentityApiEndpoints<User>().AddEntityFrameworkStores<AppDbContext>();
        builder.Services.AddAuthorization();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.ExecuteSqlRaw("CREATE EXTENSION IF NOT EXISTS citext;");
            db.Database.Migrate();
        }

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exception = context.Features
                    .Get<IExceptionHandlerFeature>()?
                    .Error;

                context.Response.ContentType = "application/json";

                if (exception is InexistentUserException)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = exception.Message
                    });
                }

                else if (exception is InexistentProjectException
                      || exception is InexistentTaskException
                      || exception is InexistentTagException
                      || exception is Services.Exceptions.UnauthorizedAccessException)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "User is not authorized"
                    });
                }

                else if (exception is UnexpectedProjectStatusException
                      || exception is UnexpectedTaskStatusException
                      || exception is UnexpectedTaskPriorityException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = exception.Message
                    });
                }

                else if (exception is DuplicateUsernameException
                      || exception is DuplicateEmailException
                      || exception is DuplicateProjectException
                      || exception is DuplicateTaskException
                      || exception is DuplicateTagException)
                {
                    context.Response.StatusCode = StatusCodes.Status409Conflict;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = exception.Message
                    });
                }

                else
                {
                    context.Response.StatusCode =
                        StatusCodes.Status500InternalServerError;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "An unexpected error occurred"
                    });
                }
            });
        });

        app.UseHttpsRedirection();
        app.MapControllers();
        app.MapGroup("/api/auth").MapIdentityApi<User>();

        app.Run();
    }
}

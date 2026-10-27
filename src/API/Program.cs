using Infrastructure.Database;
using Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;

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
            context.Database.EnsureCreated();
        }

        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
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
                    context.Response.StatusCode =
                        StatusCodes.Status404NotFound;

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
        app.Run();
    }
}

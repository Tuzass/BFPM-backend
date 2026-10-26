using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

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

        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}

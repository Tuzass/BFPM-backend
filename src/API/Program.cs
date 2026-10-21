using Infrastructure.Database;
using Services;
using Services.Exceptions;

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

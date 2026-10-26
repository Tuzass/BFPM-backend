using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public class AppDbContextFactory
{
    static public string ConnectionString = null!;

    static public AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;
        
        var context = new AppDbContext(options);
        return context;
    }
}

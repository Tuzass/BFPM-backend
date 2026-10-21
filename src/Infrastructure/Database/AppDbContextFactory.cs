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
        
        return new AppDbContext(options);
    }
}

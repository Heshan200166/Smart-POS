using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartPOS.Data;

/// <summary>
/// Factory for creating DbContext at design time (for migrations)
/// </summary>
public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Use SQLite for development
        optionsBuilder.UseSqlite("Data Source=SmartPOS.db");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}

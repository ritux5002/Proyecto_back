using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MiApp.Infrastructure.Persistence;

namespace MiApp.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=ecommerce.db")
            .Options;

        return new ApplicationDbContext(options);
    }
}
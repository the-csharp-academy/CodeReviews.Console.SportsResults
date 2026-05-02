using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NBANotifier.Models;
using NBANotifier.Services;

namespace NBANotifier.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Subscriber> Subscribers { get; set; }
}

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(Utils.GetDatabaseConnectionString());
        return new AppDbContext(optionsBuilder.Options);
    }
}
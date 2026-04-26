using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

namespace WebApi.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<AntifraudResult> AntifraudResults => Set<AntifraudResult>();
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(AppDbContext))!);
    }
}
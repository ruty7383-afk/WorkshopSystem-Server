using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using WorkshopSystem.Domain.Entities;

namespace WorkshopSystem.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Workshop> Workshops => Set<Workshop>();
    public DbSet<Registration> Registrations => Set<Registration>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker
                     .Entries<Workshop>()
                     .Where(e => e.State == EntityState.Modified))
        {
            entry.Entity.Version = Guid.NewGuid();
        }

        return base.SaveChangesAsync(ct);
    }
}
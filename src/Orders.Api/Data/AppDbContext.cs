using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Orders.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(o => o.Id);
            // Unique index ensures duplicate Kafka events don't create duplicate rows
            e.HasIndex(o => o.OrderId).IsUnique();
            e.Property(o => o.Amount).HasPrecision(18, 2);
        });
    }
}

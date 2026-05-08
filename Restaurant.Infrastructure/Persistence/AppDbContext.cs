using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;

namespace Restaurant.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<OrderHistory> OrderHistories { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Table> Tables { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Dish> Dishes { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
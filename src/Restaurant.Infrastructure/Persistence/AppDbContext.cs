using Auth.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Restaurant.Domain.Entities;
using Restaurant.Infrastructure.Persistence.Internal;
using Restaurant.Mediator.Helper.Persistence;

namespace Restaurant.Infrastructure.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
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
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<UserGroupRelation> UserGroupRelations { get; set; }    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(InfrastructureRef.Assembly);
    }


    /// <inheritdoc cref="IUnitOfWork.SaveChangesAsync"/>
    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc cref="IUnitOfWork.BeginTransactionAsync(CancellationToken)"/>
    async Task<ITransaction> IUnitOfWork.BeginTransactionAsync(CancellationToken cancellationToken)
    {
        var transaction = await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        return new EfCoreTransactionProxy(transaction);
    }

    public void Migrate()
    {
        Database.Migrate();
    }
}
using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Persistence.Internal;
using Restaurant.Mediator.Helper.Persistence;

namespace NotificationService.Infrastructure.Persistence;

public sealed class NotificationDbContext : DbContext, IUnitOfWork
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Message).HasMaxLength(1000);
        });
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
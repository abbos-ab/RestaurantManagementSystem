using Microsoft.EntityFrameworkCore.Storage;
using Restaurant.Mediator.Helper.Persistence;

namespace NotificationService.Infrastructure.Persistence.Internal;

/// <summary>
/// Реализация транзакции для EF Core.
/// </summary>
internal sealed class EfCoreTransactionProxy : ITransaction
{
    private readonly IDbContextTransaction _transaction;

    public EfCoreTransactionProxy(IDbContextTransaction transaction)
    {
        _transaction = transaction;
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return _transaction.CommitAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return _transaction.RollbackAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return _transaction.DisposeAsync();
    }

    public void Dispose()
    {
        _transaction.Dispose();
    }
}
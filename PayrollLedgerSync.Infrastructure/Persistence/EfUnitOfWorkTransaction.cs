using Microsoft.EntityFrameworkCore.Storage;
using PayrollLedgerSync.Application.Common.Interfaces;

namespace PayrollLedgerSync.Infrastructure.Persistence;

internal sealed class EfUnitOfWorkTransaction(IDbContextTransaction transaction) : IUnitOfWorkTransaction
{
    public Task CommitAsync(CancellationToken cancellationToken)
    {
        return transaction.CommitAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken)
    {
        return transaction.RollbackAsync(cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        return transaction.DisposeAsync();
    }
}

namespace PayrollLedgerSync.Application.Common.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}

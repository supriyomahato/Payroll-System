namespace PayrollLedgerSync.Application.Common.Interfaces;

/// <summary>
/// Represents a database transaction boundary for atomic commits.
/// </summary>
public interface IUnitOfWorkTransaction : IAsyncDisposable
{
    Task CommitAsync(CancellationToken cancellationToken);

    Task RollbackAsync(CancellationToken cancellationToken);
}

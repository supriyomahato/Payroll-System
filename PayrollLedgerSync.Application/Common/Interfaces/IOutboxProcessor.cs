namespace PayrollLedgerSync.Application.Common.Interfaces;

/// <summary>
/// Processes unpublished outbox events (read, publish, mark processed/failed).
/// </summary>
public interface IOutboxProcessor
{
    Task<int> ProcessPendingEventsAsync(CancellationToken cancellationToken);
}

using PayrollLedgerSync.Domain.Common;

namespace PayrollLedgerSync.Domain.Events;

/// <summary>
/// Raised when payroll batch is successfully synced to ledger.
/// </summary>
public sealed record PayrollBatchSyncedDomainEvent(Guid PayrollBatchId, DateTime OccurredOnUtc) : IDomainEvent;

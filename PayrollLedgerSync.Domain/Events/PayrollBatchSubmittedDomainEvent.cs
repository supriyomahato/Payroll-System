using PayrollLedgerSync.Domain.Common;

namespace PayrollLedgerSync.Domain.Events;

/// <summary>
/// Raised when a payroll batch enters synchronization workflow.
/// </summary>
public sealed record PayrollBatchSubmittedDomainEvent(
    Guid PayrollBatchId,
    string Period,
    decimal NetAmount,
    DateTime OccurredOnUtc) : IDomainEvent;

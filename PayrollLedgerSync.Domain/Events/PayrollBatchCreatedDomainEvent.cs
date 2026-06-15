using PayrollLedgerSync.Domain.Common;

namespace PayrollLedgerSync.Domain.Events;

/// <summary>
/// Raised when a new payroll batch aggregate is created.
/// </summary>
public sealed record PayrollBatchCreatedDomainEvent(
    Guid PayrollBatchId,
    string Period,
    DateTime OccurredOnUtc) : IDomainEvent;

using PayrollLedgerSync.Domain.Common;

namespace PayrollLedgerSync.Domain.Events;

/// <summary>
/// Raised when an employee payroll line is added.
/// </summary>
public sealed record EmployeePayrollAddedDomainEvent(
    Guid PayrollBatchId,
    Guid EmployeePayrollId,
    string EmployeeCode,
    decimal NetPay,
    DateTime OccurredOnUtc) : IDomainEvent;

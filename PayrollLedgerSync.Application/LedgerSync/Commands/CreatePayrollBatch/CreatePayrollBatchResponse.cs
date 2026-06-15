using PayrollLedgerSync.Domain.Entities;

namespace PayrollLedgerSync.Application.LedgerSync.Commands.CreatePayrollBatch;

/// <summary>
/// Response returned after a payroll batch is created.
/// </summary>
public sealed record CreatePayrollBatchResponse
{
    public required Guid PayrollBatchId { get; init; }

    public required string Period { get; init; }

    public required PayrollBatchStatus Status { get; init; }

    public required DateTime CreatedOnUtc { get; init; }

    public required Guid OutboxEventId { get; init; }
}

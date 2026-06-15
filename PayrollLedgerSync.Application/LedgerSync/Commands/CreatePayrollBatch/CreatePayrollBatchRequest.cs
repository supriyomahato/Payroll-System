namespace PayrollLedgerSync.Application.LedgerSync.Commands.CreatePayrollBatch;

/// <summary>
/// Request payload for creating a payroll batch.
/// </summary>
public sealed record CreatePayrollBatchRequest
{
    public required string Period { get; init; }

    public required string CreatedBy { get; init; }

    public string Currency { get; init; } = "USD";

    public IReadOnlyCollection<CreateEmployeePayrollLineRequest> EmployeeLines { get; init; } =
        Array.Empty<CreateEmployeePayrollLineRequest>();
}

public sealed record CreateEmployeePayrollLineRequest
{
    public required string EmployeeCode { get; init; }

    public decimal GrossAmount { get; init; }

    public decimal DeductionAmount { get; init; }
}

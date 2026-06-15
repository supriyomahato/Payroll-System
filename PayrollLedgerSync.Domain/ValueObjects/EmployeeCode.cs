namespace PayrollLedgerSync.Domain.ValueObjects;

/// <summary>
/// Domain identifier for employee payroll processing.
/// </summary>
public sealed record EmployeeCode
{
    private EmployeeCode()
    {
        Value = string.Empty;
    }

    public string Value { get; }

    private EmployeeCode(string value)
    {
        Value = value;
    }

    public static EmployeeCode Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Length > 32)
        {
            throw new ArgumentException("Employee code is required and max 32 chars.", nameof(value));
        }

        return new EmployeeCode(value.Trim().ToUpperInvariant());
    }

    public override string ToString() => Value;
}

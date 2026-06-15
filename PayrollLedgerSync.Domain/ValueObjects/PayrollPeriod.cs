namespace PayrollLedgerSync.Domain.ValueObjects;

/// <summary>
/// Payroll period represented as Year-Month.
/// </summary>
public sealed record PayrollPeriod
{
    private PayrollPeriod()
    {
        Year = default;
        Month = default;
    }

    public int Year { get; }
    public int Month { get; }

    private PayrollPeriod(int year, int month)
    {
        Year = year;
        Month = month;
    }

    public static PayrollPeriod Of(int year, int month)
    {
        if (year < 2000 || year > 2100)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year out of supported range.");
        }

        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be 1..12.");
        }

        return new PayrollPeriod(year, month);
    }

    public static PayrollPeriod Parse(string period)
    {
        if (string.IsNullOrWhiteSpace(period))
        {
            throw new ArgumentException("Period is required.", nameof(period));
        }

        var parts = period.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2 || !int.TryParse(parts[0], out var year) || !int.TryParse(parts[1], out var month))
        {
            throw new ArgumentException("Period format must be yyyy-MM.", nameof(period));
        }

        return Of(year, month);
    }

    public override string ToString() => $"{Year:D4}-{Month:D2}";
}

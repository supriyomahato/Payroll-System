namespace PayrollLedgerSync.Domain.ValueObjects;

/// <summary>
/// Money value object with amount and currency.
/// Business rule: payroll and ledger amounts cannot use mixed currencies inside one batch.
/// </summary>
public sealed record Money
{
    private Money()
    {
        Amount = default;
        Currency = string.Empty;
    }

    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Of(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
        {
            throw new ArgumentException("Currency must be ISO-4217 3-letter code.", nameof(currency));
        }

        return new Money(decimal.Round(amount, 2, MidpointRounding.AwayFromZero), currency.Trim().ToUpperInvariant());
    }

    public static Money Zero(string currency) => Of(0, currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return Of(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        if (other.Amount > Amount)
        {
            throw new InvalidOperationException("Cannot subtract a larger amount.");
        }

        return Of(Amount - other.Amount, Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (!string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Money operations require same currency.");
        }
    }
}

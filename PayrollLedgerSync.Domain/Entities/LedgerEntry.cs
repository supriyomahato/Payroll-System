using PayrollLedgerSync.Domain.ValueObjects;

namespace PayrollLedgerSync.Domain.Entities;

/// <summary>
/// Child entity under PayrollBatch aggregate.
/// Business rule: a ledger line must be either debit OR credit (never both).
/// </summary>
public sealed class LedgerEntry
{
    private LedgerEntry()
    {
        AccountCode = string.Empty;
        Debit = Money.Zero("USD");
        Credit = Money.Zero("USD");
        Description = string.Empty;
    }

    public Guid Id { get; private set; }
    public string AccountCode { get; private set; }
    public Money Debit { get; private set; }
    public Money Credit { get; private set; }
    public string Description { get; private set; }

    private LedgerEntry(Guid id, string accountCode, Money debit, Money credit, string description)
    {
        if (debit.Currency != credit.Currency)
        {
            throw new InvalidOperationException("Debit and credit currencies must match.");
        }

        var debitPositive = debit.Amount > 0;
        var creditPositive = credit.Amount > 0;
        if (debitPositive == creditPositive)
        {
            throw new InvalidOperationException("Ledger entry must have exactly one positive side (debit or credit).");
        }

        if (string.IsNullOrWhiteSpace(accountCode))
        {
            throw new ArgumentException("Account code is required.", nameof(accountCode));
        }

        Id = id;
        AccountCode = accountCode.Trim().ToUpperInvariant();
        Debit = debit;
        Credit = credit;
        Description = string.IsNullOrWhiteSpace(description) ? "Payroll posting" : description.Trim();
    }

    public static LedgerEntry DebitLine(string accountCode, Money amount, string description)
    {
        return new LedgerEntry(Guid.NewGuid(), accountCode, amount, Money.Zero(amount.Currency), description);
    }

    public static LedgerEntry CreditLine(string accountCode, Money amount, string description)
    {
        return new LedgerEntry(Guid.NewGuid(), accountCode, Money.Zero(amount.Currency), amount, description);
    }
}

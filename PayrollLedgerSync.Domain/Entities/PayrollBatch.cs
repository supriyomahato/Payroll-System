using PayrollLedgerSync.Domain.Common;
using PayrollLedgerSync.Domain.Events;
using PayrollLedgerSync.Domain.ValueObjects;

namespace PayrollLedgerSync.Domain.Entities;

public enum PayrollBatchStatus
{
    Draft = 1,
    Submitted = 2,
    Synced = 3
}

/// <summary>
/// Aggregate root for payroll processing and ledger synchronization.
/// Business rules:
/// 1) Employee lines are mutable only while batch is Draft.
/// 2) Batch must be balanced before submission (debit total == credit total).
/// 3) Once synced, batch becomes immutable.
/// </summary>
public sealed class PayrollBatch : AggregateRoot
{
    private readonly List<EmployeePayroll> _employeePayrolls = [];
    private readonly List<LedgerEntry> _ledgerEntries = [];

    private PayrollBatch()
    {
        PayrollPeriod = PayrollPeriod.Of(2000, 1);
    }

    public PayrollPeriod PayrollPeriod { get; private set; }
    public PayrollBatchStatus Status { get; private set; }

    public string Period => PayrollPeriod.ToString();
    public decimal NetAmount => _employeePayrolls.Sum(x => x.NetPay.Amount);
    public bool IsSyncedToLedger => Status == PayrollBatchStatus.Synced;

    public IReadOnlyCollection<EmployeePayroll> EmployeePayrolls => _employeePayrolls.AsReadOnly();
    public IReadOnlyCollection<LedgerEntry> LedgerEntries => _ledgerEntries.AsReadOnly();

    private PayrollBatch(Guid id, PayrollPeriod payrollPeriod, string createdBy)
    {
        Id = id;
        PayrollPeriod = payrollPeriod;
        Status = PayrollBatchStatus.Draft;
        SetCreatedAudit(createdBy, DateTime.UtcNow);
    }

    public static PayrollBatch Create(string period, decimal netAmount)
    {
        // Backward-compatible factory for existing application code.
        var batch = new PayrollBatch(Guid.NewGuid(), PayrollPeriod.Parse(period), "system");

        // Represent migrated/legacy net as one employee line when detailed lines are unavailable.
        if (netAmount > 0)
        {
            batch.AddEmployeePayroll(EmployeeCode.Of("LEGACY"), Money.Of(netAmount, "USD"), Money.Zero("USD"), "system");
        }

        return batch;
    }

    public static PayrollBatch Create(PayrollPeriod payrollPeriod, string createdBy)
    {
        var batch = new PayrollBatch(Guid.NewGuid(), payrollPeriod, createdBy);
        batch.AddDomainEvent(new PayrollBatchCreatedDomainEvent(batch.Id, batch.Period, DateTime.UtcNow));
        return batch;
    }

    public void AddEmployeePayroll(EmployeeCode employeeCode, Money grossPay, Money deductions, string performedBy)
    {
        EnsureDraft();

        var employeePayroll = EmployeePayroll.Create(employeeCode, grossPay, deductions);
        _employeePayrolls.Add(employeePayroll);

        Touch(performedBy, DateTime.UtcNow);
        AddDomainEvent(new EmployeePayrollAddedDomainEvent(Id, employeePayroll.Id, employeeCode.Value, employeePayroll.NetPay.Amount, DateTime.UtcNow));
    }

    public void AddLedgerEntryDebit(string accountCode, Money amount, string description, string performedBy)
    {
        EnsureDraft();

        _ledgerEntries.Add(LedgerEntry.DebitLine(accountCode, amount, description));
        Touch(performedBy, DateTime.UtcNow);
    }

    public void AddLedgerEntryCredit(string accountCode, Money amount, string description, string performedBy)
    {
        EnsureDraft();

        _ledgerEntries.Add(LedgerEntry.CreditLine(accountCode, amount, description));
        Touch(performedBy, DateTime.UtcNow);
    }

    public void SubmitForSynchronization(string performedBy)
    {
        EnsureDraft();

        if (_employeePayrolls.Count == 0)
        {
            throw new InvalidOperationException("At least one employee payroll line is required before submission.");
        }

        if (_ledgerEntries.Count == 0)
        {
            throw new InvalidOperationException("Ledger entries are required before submission.");
        }

        var debit = _ledgerEntries.Sum(x => x.Debit.Amount);
        var credit = _ledgerEntries.Sum(x => x.Credit.Amount);
        if (debit != credit)
        {
            throw new InvalidOperationException("Batch cannot be submitted when ledger is not balanced.");
        }

        Status = PayrollBatchStatus.Submitted;
        Touch(performedBy, DateTime.UtcNow);
        AddDomainEvent(new PayrollBatchSubmittedDomainEvent(Id, Period, NetAmount, DateTime.UtcNow));
    }

    public void MarkAsSynced()
    {
        if (Status == PayrollBatchStatus.Synced)
        {
            return;
        }

        // Business rule: normal flow is Draft -> Submitted -> Synced.
        // Compatibility rule: legacy integrations can sync directly from Draft.
        if (Status == PayrollBatchStatus.Draft)
        {
            Status = PayrollBatchStatus.Submitted;
            AddDomainEvent(new PayrollBatchSubmittedDomainEvent(Id, Period, NetAmount, DateTime.UtcNow));
        }

        Status = PayrollBatchStatus.Synced;
        Touch("system", DateTime.UtcNow);
        AddDomainEvent(new PayrollBatchSyncedDomainEvent(Id, DateTime.UtcNow));
    }

    private void EnsureDraft()
    {
        if (Status != PayrollBatchStatus.Draft)
        {
            throw new InvalidOperationException("Batch can be changed only in Draft state.");
        }
    }
}

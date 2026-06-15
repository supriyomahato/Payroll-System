using PayrollLedgerSync.Domain.ValueObjects;

namespace PayrollLedgerSync.Domain.Entities;

/// <summary>
/// Child entity under PayrollBatch aggregate.
/// Business rule: net pay = gross - deductions and cannot be negative.
/// </summary>
public sealed class EmployeePayroll
{
    private EmployeePayroll()
    {
        EmployeeCode = EmployeeCode.Of("UNKNOWN");
        GrossPay = Money.Zero("USD");
        Deductions = Money.Zero("USD");
        NetPay = Money.Zero("USD");
    }

    public Guid Id { get; private set; }
    public EmployeeCode EmployeeCode { get; private set; }
    public Money GrossPay { get; private set; }
    public Money Deductions { get; private set; }
    public Money NetPay { get; private set; }

    private EmployeePayroll(Guid id, EmployeeCode employeeCode, Money grossPay, Money deductions)
    {
        if (grossPay.Currency != deductions.Currency)
        {
            throw new InvalidOperationException("Gross and deductions must use same currency.");
        }

        if (deductions.Amount > grossPay.Amount)
        {
            throw new InvalidOperationException("Deductions cannot exceed gross pay.");
        }

        Id = id;
        EmployeeCode = employeeCode;
        GrossPay = grossPay;
        Deductions = deductions;
        NetPay = grossPay.Subtract(deductions);
    }

    public static EmployeePayroll Create(EmployeeCode employeeCode, Money grossPay, Money deductions)
    {
        return new EmployeePayroll(Guid.NewGuid(), employeeCode, grossPay, deductions);
    }
}

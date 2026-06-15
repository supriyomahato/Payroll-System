using FluentValidation;
using PayrollLedgerSync.Domain.ValueObjects;

namespace PayrollLedgerSync.Application.LedgerSync.Commands.CreatePayrollBatch;

public sealed class CreatePayrollBatchCommandValidator : AbstractValidator<CreatePayrollBatchCommand>
{
    public CreatePayrollBatchCommandValidator()
    {
        RuleFor(x => x.Request)
            .NotNull();

        RuleFor(x => x.Request.Period)
            .NotEmpty()
            .Must(BeValidPeriod)
            .WithMessage("Period must be in yyyy-MM format.");

        RuleFor(x => x.Request.CreatedBy)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Request.Currency)
            .NotEmpty()
            .Length(3);

        RuleForEach(x => x.Request.EmployeeLines)
            .ChildRules(line =>
            {
                line.RuleFor(x => x.EmployeeCode)
                    .NotEmpty()
                    .MaximumLength(32);

                line.RuleFor(x => x.GrossAmount)
                    .GreaterThanOrEqualTo(0);

                line.RuleFor(x => x.DeductionAmount)
                    .GreaterThanOrEqualTo(0);

                line.RuleFor(x => x)
                    .Must(x => x.DeductionAmount <= x.GrossAmount)
                    .WithMessage("Deductions cannot exceed gross pay.");
            });

        RuleFor(x => x.Request.EmployeeLines)
            .Must(lines => lines.Select(x => x.EmployeeCode.Trim().ToUpperInvariant()).Distinct().Count() == lines.Count)
            .When(x => x.Request.EmployeeLines.Count > 0)
            .WithMessage("Employee codes must be unique within the batch.");
    }

    private static bool BeValidPeriod(string period)
    {
        try
        {
            PayrollPeriod.Parse(period);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

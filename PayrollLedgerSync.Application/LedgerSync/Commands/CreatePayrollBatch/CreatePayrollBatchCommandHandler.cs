using System.Text.Json;
using MediatR;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Domain.Entities;
using PayrollLedgerSync.Domain.Events;
using PayrollLedgerSync.Domain.ValueObjects;

namespace PayrollLedgerSync.Application.LedgerSync.Commands.CreatePayrollBatch;

public sealed class CreatePayrollBatchCommandHandler(
    IPayrollBatchRepository payrollBatchRepository,
    IOutboxRepository outboxRepository,
    IUnitOfWork unitOfWork,
    IDomainEventDispatcher domainEventDispatcher)
    : IRequestHandler<CreatePayrollBatchCommand, CreatePayrollBatchResponse>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<CreatePayrollBatchResponse> Handle(
        CreatePayrollBatchCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var payrollPeriod = PayrollPeriod.Parse(request.Period);

        if (await payrollBatchRepository.ExistsForPeriodAsync(payrollPeriod.ToString(), cancellationToken))
        {
            throw new InvalidOperationException($"A payroll batch already exists for period '{payrollPeriod}'.");
        }

        var payrollBatch = PayrollBatch.Create(payrollPeriod, request.CreatedBy);

        foreach (var line in request.EmployeeLines)
        {
            payrollBatch.AddEmployeePayroll(
                EmployeeCode.Of(line.EmployeeCode),
                Money.Of(line.GrossAmount, request.Currency),
                Money.Of(line.DeductionAmount, request.Currency),
                request.CreatedBy);
        }

        var createdDomainEvent = payrollBatch.DomainEvents
            .OfType<PayrollBatchCreatedDomainEvent>()
            .Single();

        var outboxPayload = JsonSerializer.Serialize(new
        {
            createdDomainEvent.PayrollBatchId,
            createdDomainEvent.Period,
            createdDomainEvent.OccurredOnUtc
        }, JsonOptions);

        var outboxEvent = OutboxEvent.Create(
            nameof(PayrollBatchCreatedDomainEvent),
            outboxPayload,
            createdDomainEvent.OccurredOnUtc,
            request.CreatedBy);

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await payrollBatchRepository.AddAsync(payrollBatch, cancellationToken);
            await outboxRepository.AddAsync(outboxEvent, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        var domainEvents = payrollBatch.DequeueDomainEvents();
        await domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);

        return new CreatePayrollBatchResponse
        {
            PayrollBatchId = payrollBatch.Id,
            Period = payrollBatch.Period,
            Status = payrollBatch.Status,
            CreatedOnUtc = payrollBatch.CreatedOnUtc,
            OutboxEventId = outboxEvent.Id
        };
    }
}

using MediatR;
using Microsoft.Extensions.Logging;
using PayrollLedgerSync.Domain.Events;

namespace PayrollLedgerSync.Application.LedgerSync.EventHandlers;

public sealed class PayrollBatchCreatedDomainEventHandler(ILogger<PayrollBatchCreatedDomainEventHandler> logger)
    : INotificationHandler<PayrollBatchCreatedDomainEvent>
{
    public Task Handle(PayrollBatchCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Payroll batch {PayrollBatchId} created for period {Period} at {OccurredOnUtc}.",
            notification.PayrollBatchId,
            notification.Period,
            notification.OccurredOnUtc);

        return Task.CompletedTask;
    }
}

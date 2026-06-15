using MediatR;
using Microsoft.Extensions.Logging;
using PayrollLedgerSync.Domain.Events;

namespace PayrollLedgerSync.Application.LedgerSync.EventHandlers;

public sealed class PayrollBatchSyncedDomainEventHandler(ILogger<PayrollBatchSyncedDomainEventHandler> logger)
    : INotificationHandler<PayrollBatchSyncedDomainEvent>
{
    public Task Handle(PayrollBatchSyncedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Payroll batch {PayrollBatchId} synced at {OccurredOnUtc}.", notification.PayrollBatchId, notification.OccurredOnUtc);
        return Task.CompletedTask;
    }
}

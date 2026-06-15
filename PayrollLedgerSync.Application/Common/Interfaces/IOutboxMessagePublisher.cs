using PayrollLedgerSync.Domain.Entities;

namespace PayrollLedgerSync.Application.Common.Interfaces;

/// <summary>
/// Publishes outbox messages to external message brokers.
/// </summary>
public interface IOutboxMessagePublisher
{
    Task PublishAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken);
}

using MediatR;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Domain.Common;

namespace PayrollLedgerSync.Application.Common.Services;

public sealed class DomainEventDispatcher(IPublisher publisher) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent, cancellationToken);
        }
    }
}

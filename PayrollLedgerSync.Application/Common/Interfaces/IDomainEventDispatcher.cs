using PayrollLedgerSync.Domain.Common;

namespace PayrollLedgerSync.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken);
}

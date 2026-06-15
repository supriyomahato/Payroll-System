using MediatR;

namespace PayrollLedgerSync.Domain.Common;

/// <summary>
/// Marker interface for all domain events.
/// </summary>
public interface IDomainEvent : INotification
{
    DateTime OccurredOnUtc { get; }
}

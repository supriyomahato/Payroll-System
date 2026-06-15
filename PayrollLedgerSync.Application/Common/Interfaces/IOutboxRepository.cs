using PayrollLedgerSync.Domain.Entities;

namespace PayrollLedgerSync.Application.Common.Interfaces;

/// <summary>
/// Persistence port for transactional outbox read/write operations.
/// </summary>
public interface IOutboxRepository
{
    Task AddAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken);

    Task<IReadOnlyList<OutboxEvent>> GetUnpublishedAsync(int batchSize, CancellationToken cancellationToken);

    Task<OutboxEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task MarkProcessedAsync(Guid outboxEventId, string processedBy, CancellationToken cancellationToken);

    Task RegisterFailureAsync(Guid outboxEventId, string error, string processedBy, CancellationToken cancellationToken);
}

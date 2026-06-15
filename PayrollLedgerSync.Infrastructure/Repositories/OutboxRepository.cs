using Microsoft.EntityFrameworkCore;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Domain.Entities;
using PayrollLedgerSync.Infrastructure.Persistence;

namespace PayrollLedgerSync.Infrastructure.Repositories;

public sealed class OutboxRepository(PayrollLedgerDbContext dbContext) : IOutboxRepository
{
    public async Task AddAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken)
    {
        await dbContext.OutboxEvents.AddAsync(outboxEvent, cancellationToken);
    }

    public async Task<IReadOnlyList<OutboxEvent>> GetUnpublishedAsync(int batchSize, CancellationToken cancellationToken)
    {
        return await dbContext.OutboxEvents
            .Where(outboxEvent => outboxEvent.ProcessedOnUtc == null)
            .OrderBy(outboxEvent => outboxEvent.OccurredOnUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public Task<OutboxEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.OutboxEvents.FirstOrDefaultAsync(outboxEvent => outboxEvent.Id == id, cancellationToken);
    }

    public async Task MarkProcessedAsync(Guid outboxEventId, string processedBy, CancellationToken cancellationToken)
    {
        var outboxEvent = await GetTrackedAsync(outboxEventId, cancellationToken);
        if (outboxEvent is null)
        {
            return;
        }

        outboxEvent.MarkProcessed(processedBy);
    }

    public async Task RegisterFailureAsync(
        Guid outboxEventId,
        string error,
        string processedBy,
        CancellationToken cancellationToken)
    {
        var outboxEvent = await GetTrackedAsync(outboxEventId, cancellationToken);
        if (outboxEvent is null)
        {
            return;
        }

        outboxEvent.RegisterPublishFailure(error, processedBy);
    }

    private Task<OutboxEvent?> GetTrackedAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.OutboxEvents.FirstOrDefaultAsync(outboxEvent => outboxEvent.Id == id, cancellationToken);
    }
}

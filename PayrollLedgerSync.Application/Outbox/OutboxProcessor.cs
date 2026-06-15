using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Application.Common.Options;

namespace PayrollLedgerSync.Application.Outbox;

public sealed class OutboxProcessor(
    IOutboxRepository outboxRepository,
    IOutboxMessagePublisher messagePublisher,
    IUnitOfWork unitOfWork,
    IOptions<OutboxProcessorOptions> options,
    ILogger<OutboxProcessor> logger) : IOutboxProcessor
{
    public async Task<int> ProcessPendingEventsAsync(CancellationToken cancellationToken)
    {
        var settings = options.Value;
        var unpublishedEvents = await outboxRepository.GetUnpublishedAsync(settings.BatchSize, cancellationToken);

        if (unpublishedEvents.Count == 0)
        {
            return 0;
        }

        var processedCount = 0;

        foreach (var outboxEvent in unpublishedEvents)
        {
            if (outboxEvent.RetryCount >= settings.MaxRetryCount)
            {
                logger.LogError(
                    "Outbox event {OutboxEventId} exceeded max retry count {MaxRetryCount}. Last error: {Error}",
                    outboxEvent.Id,
                    settings.MaxRetryCount,
                    outboxEvent.Error);

                continue;
            }

            try
            {
                await messagePublisher.PublishAsync(outboxEvent, cancellationToken);
                await outboxRepository.MarkProcessedAsync(outboxEvent.Id, settings.ProcessedBy, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                processedCount++;

                logger.LogInformation(
                    "Published outbox event {OutboxEventId} of type {EventType}.",
                    outboxEvent.Id,
                    outboxEvent.EventType);
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "Failed to publish outbox event {OutboxEventId} of type {EventType}. RetryCount={RetryCount}",
                    outboxEvent.Id,
                    outboxEvent.EventType,
                    outboxEvent.RetryCount);

                await outboxRepository.RegisterFailureAsync(
                    outboxEvent.Id,
                    exception.Message,
                    settings.ProcessedBy,
                    cancellationToken);

                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return processedCount;
    }
}

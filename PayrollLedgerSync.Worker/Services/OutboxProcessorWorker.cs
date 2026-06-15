using Microsoft.Extensions.Options;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Application.Common.Options;

namespace PayrollLedgerSync.Worker.Services;

/// <summary>
/// Background worker that polls the transactional outbox and publishes events to RabbitMQ.
/// </summary>
public sealed class OutboxProcessorWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<OutboxProcessorOptions> options,
    ILogger<OutboxProcessorWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;

        logger.LogInformation(
            "Outbox processor worker started. Poll interval={PollingIntervalSeconds}s, batch size={BatchSize}, max retries={MaxRetryCount}.",
            settings.PollingIntervalSeconds,
            settings.BatchSize,
            settings.MaxRetryCount);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
                var processedCount = await processor.ProcessPendingEventsAsync(stoppingToken);

                if (processedCount > 0)
                {
                    logger.LogInformation("Outbox processor published {ProcessedCount} event(s).", processedCount);
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(exception, "Unhandled exception in outbox processor worker loop.");
            }

            await Task.Delay(TimeSpan.FromSeconds(settings.PollingIntervalSeconds), stoppingToken);
        }

        logger.LogInformation("Outbox processor worker stopped.");
    }
}

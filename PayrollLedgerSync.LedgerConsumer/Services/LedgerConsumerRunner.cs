using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PayrollLedgerSync.Application.LedgerSync.Commands;
using PayrollLedgerSync.Application.LedgerSync.Queries;

namespace PayrollLedgerSync.LedgerConsumer.Services;

public sealed class LedgerConsumerRunner(IServiceProvider serviceProvider, ILogger<LedgerConsumerRunner> logger)
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var pending = await sender.Send(new GetPendingSyncBatchesQuery(), cancellationToken);

        foreach (var batch in pending)
        {
            var synced = await sender.Send(new SyncPayrollBatchCommand(batch.PayrollBatchId), cancellationToken);
            logger.LogInformation("Consumer sync for batch {PayrollBatchId}: {Synced}", batch.PayrollBatchId, synced);
        }
    }
}

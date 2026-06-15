using MediatR;
using PayrollLedgerSync.Application.LedgerSync.Commands;
using PayrollLedgerSync.Application.LedgerSync.Queries;

namespace PayrollLedgerSync.Worker.Services;

public sealed class PayrollSyncBackgroundService(IServiceScopeFactory scopeFactory, ILogger<PayrollSyncBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            var pending = await sender.Send(new GetPendingSyncBatchesQuery(), stoppingToken);
            foreach (var batch in pending)
            {
                await sender.Send(new SyncPayrollBatchCommand(batch.PayrollBatchId), stoppingToken);
                logger.LogInformation("Worker synchronized payroll batch {PayrollBatchId}.", batch.PayrollBatchId);
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}

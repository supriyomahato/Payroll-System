using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Application.DependencyInjection;
using PayrollLedgerSync.Application.LedgerSync.Commands;
using PayrollLedgerSync.Application.LedgerSync.Queries;
using PayrollLedgerSync.Domain.Entities;
using PayrollLedgerSync.Infrastructure.Persistence;
using PayrollLedgerSync.Infrastructure.Repositories;

namespace PayrollLedgerSync.Tests.Application;

public sealed class SyncPayrollBatchCommandTests
{
    [Fact]
    public async Task SyncPayrollBatchCommand_Should_MarkBatchAsSynced()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();

        services.AddDbContext<PayrollLedgerDbContext>(options =>
            options.UseInMemoryDatabase($"PayrollLedgerSyncTests-{Guid.NewGuid()}"));
        services.AddScoped<IPayrollBatchRepository, PayrollBatchRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<PayrollLedgerDbContext>();
        var batch = PayrollBatch.Create("2026-06", 5000m);
        await dbContext.PayrollBatches.AddAsync(batch);
        await dbContext.SaveChangesAsync();

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var result = await sender.Send(new SyncPayrollBatchCommand(batch.Id));
        var pendingAfter = await sender.Send(new GetPendingSyncBatchesQuery());

        Assert.True(result);
        Assert.DoesNotContain(pendingAfter, item => item.PayrollBatchId == batch.Id);
    }
}

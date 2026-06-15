using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Application.DependencyInjection;
using PayrollLedgerSync.Application.LedgerSync.Commands.CreatePayrollBatch;
using PayrollLedgerSync.Domain.Entities;
using PayrollLedgerSync.Infrastructure.Persistence;
using PayrollLedgerSync.Infrastructure.Repositories;

namespace PayrollLedgerSync.Tests.Application;

public sealed class CreatePayrollBatchCommandTests
{
    [Fact]
    public async Task CreatePayrollBatchCommand_Should_PersistBatchAndOutboxInSingleTransaction()
    {
        var services = BuildServices();
        using var scope = services.CreateScope();

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var dbContext = scope.ServiceProvider.GetRequiredService<PayrollLedgerDbContext>();

        var request = new CreatePayrollBatchRequest
        {
            Period = "2026-07",
            CreatedBy = "test-user",
            Currency = "USD",
            EmployeeLines =
            [
                new CreateEmployeePayrollLineRequest
                {
                    EmployeeCode = "EMP001",
                    GrossAmount = 5000m,
                    DeductionAmount = 500m
                }
            ]
        };

        var response = await sender.Send(new CreatePayrollBatchCommand(request));

        Assert.NotEqual(Guid.Empty, response.PayrollBatchId);
        Assert.Equal("2026-07", response.Period);
        Assert.Equal(PayrollBatchStatus.Draft, response.Status);
        Assert.NotEqual(Guid.Empty, response.OutboxEventId);

        var batch = await dbContext.PayrollBatches.FindAsync(response.PayrollBatchId);
        var outbox = await dbContext.OutboxEvents.FindAsync(response.OutboxEventId);

        Assert.NotNull(batch);
        Assert.NotNull(outbox);
        Assert.Equal("PayrollBatchCreatedDomainEvent", outbox.EventType);
        Assert.Null(outbox.ProcessedOnUtc);
    }

    [Fact]
    public async Task CreatePayrollBatchCommand_Should_RejectDuplicatePeriod()
    {
        var services = BuildServices();
        using var scope = services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var request = new CreatePayrollBatchRequest
        {
            Period = "2026-08",
            CreatedBy = "test-user"
        };

        await sender.Send(new CreatePayrollBatchCommand(request));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sender.Send(new CreatePayrollBatchCommand(request)));
    }

    [Fact]
    public async Task CreatePayrollBatchCommand_Should_FailValidation_WhenPeriodInvalid()
    {
        var services = BuildServices();
        using var scope = services.CreateScope();
        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var request = new CreatePayrollBatchRequest
        {
            Period = "invalid-period",
            CreatedBy = "test-user"
        };

        await Assert.ThrowsAsync<ValidationException>(() =>
            sender.Send(new CreatePayrollBatchCommand(request)));
    }

    private static ServiceProvider BuildServices()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();

        services.AddDbContext<PayrollLedgerDbContext>(options =>
            options
                .UseInMemoryDatabase($"PayrollLedgerSyncTests-{Guid.NewGuid()}")
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning)));
        services.AddScoped<IPayrollBatchRepository, PayrollBatchRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        return services.BuildServiceProvider();
    }
}

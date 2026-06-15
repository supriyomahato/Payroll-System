using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Application.Common.Options;
using PayrollLedgerSync.Application.DependencyInjection;
using PayrollLedgerSync.Application.Outbox;
using PayrollLedgerSync.Domain.Entities;
using PayrollLedgerSync.Infrastructure.Persistence;
using PayrollLedgerSync.Infrastructure.Repositories;

namespace PayrollLedgerSync.Tests.Application;

public sealed class OutboxProcessorTests
{
    [Fact]
    public async Task ProcessPendingEventsAsync_Should_PublishAndMarkProcessed()
    {
        var publisher = new FakeOutboxMessagePublisher();
        var services = BuildServices(publisher);
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<PayrollLedgerDbContext>();
        var outboxEvent = OutboxEvent.Create("TestEvent", """{"id":"1"}""", DateTime.UtcNow, "test");
        await dbContext.OutboxEvents.AddAsync(outboxEvent);
        await dbContext.SaveChangesAsync();

        var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
        var processed = await processor.ProcessPendingEventsAsync(CancellationToken.None);

        Assert.Equal(1, processed);
        Assert.Single(publisher.PublishedEvents);

        var persisted = await dbContext.OutboxEvents.FindAsync(outboxEvent.Id);
        Assert.NotNull(persisted);
        Assert.NotNull(persisted.ProcessedOnUtc);
    }

    [Fact]
    public async Task ProcessPendingEventsAsync_Should_RegisterFailure_WhenPublishFails()
    {
        var publisher = new FakeOutboxMessagePublisher { ShouldFail = true };
        var services = BuildServices(publisher);
        using var scope = services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<PayrollLedgerDbContext>();
        var outboxEvent = OutboxEvent.Create("TestEvent", """{"id":"1"}""", DateTime.UtcNow, "test");
        await dbContext.OutboxEvents.AddAsync(outboxEvent);
        await dbContext.SaveChangesAsync();

        var processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
        var processed = await processor.ProcessPendingEventsAsync(CancellationToken.None);

        Assert.Equal(0, processed);

        var persisted = await dbContext.OutboxEvents.FindAsync(outboxEvent.Id);
        Assert.NotNull(persisted);
        Assert.Null(persisted.ProcessedOnUtc);
        Assert.Equal(1, persisted.RetryCount);
        Assert.NotNull(persisted.Error);
    }

    private static ServiceProvider BuildServices(FakeOutboxMessagePublisher publisher)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();

        services.AddDbContext<PayrollLedgerDbContext>(options =>
            options.UseInMemoryDatabase($"OutboxProcessorTests-{Guid.NewGuid()}"));

        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddSingleton<IOutboxMessagePublisher>(publisher);
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();
        services.AddSingleton(Options.Create(new OutboxProcessorOptions
        {
            BatchSize = 10,
            MaxRetryCount = 5,
            ProcessedBy = "test-processor"
        }));

        return services.BuildServiceProvider();
    }

    private sealed class FakeOutboxMessagePublisher : IOutboxMessagePublisher
    {
        public bool ShouldFail { get; set; }

        public List<OutboxEvent> PublishedEvents { get; } = [];

        public Task PublishAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken)
        {
            if (ShouldFail)
            {
                throw new InvalidOperationException("Simulated broker failure.");
            }

            PublishedEvents.Add(outboxEvent);
            return Task.CompletedTask;
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Application.Common.Options;
using PayrollLedgerSync.Application.Outbox;
using PayrollLedgerSync.Infrastructure.Messaging;
using PayrollLedgerSync.Infrastructure.Repositories;

namespace PayrollLedgerSync.Infrastructure.DependencyInjection;

public static class OutboxInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddOutboxInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.Configure<OutboxProcessorOptions>(configuration.GetSection(OutboxProcessorOptions.SectionName));

        services.AddSingleton<RabbitMqConnectionManager>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IOutboxMessagePublisher, RabbitMqOutboxMessagePublisher>();
        services.AddScoped<IOutboxProcessor, OutboxProcessor>();

        return services;
    }
}

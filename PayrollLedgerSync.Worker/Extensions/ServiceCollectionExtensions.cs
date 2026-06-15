using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayrollLedgerSync.Application.DependencyInjection;
using PayrollLedgerSync.Infrastructure.DependencyInjection;

namespace PayrollLedgerSync.Worker.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkerDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);
        return services;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Infrastructure.Persistence;
using PayrollLedgerSync.Infrastructure.Repositories;

namespace PayrollLedgerSync.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PayrollLedger")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=PayrollLedgerSyncDb;Trusted_Connection=True;TrustServerCertificate=True;";

        services.AddDbContext<PayrollLedgerDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.MigrationsAssembly(typeof(PayrollLedgerDbContext).Assembly.FullName)));

        services.AddScoped<IPayrollBatchRepository, PayrollBatchRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddOutboxInfrastructure(configuration);

        return services;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PayrollLedgerSync.Infrastructure.Persistence;

public sealed class PayrollLedgerDbContextFactory : IDesignTimeDbContextFactory<PayrollLedgerDbContext>
{
    public PayrollLedgerDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("PayrollLedger")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=PayrollLedgerSyncDb;Trusted_Connection=True;TrustServerCertificate=True;";

        var optionsBuilder = new DbContextOptionsBuilder<PayrollLedgerDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new PayrollLedgerDbContext(optionsBuilder.Options);
    }
}

using PayrollLedgerSync.Worker.Extensions;
using PayrollLedgerSync.Worker.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog((services, loggerConfiguration) =>
        loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console());

    builder.Services.AddWorkerDependencies(builder.Configuration);
    builder.Services.AddHostedService<OutboxProcessorWorker>();
    builder.Services.AddHostedService<PayrollSyncBackgroundService>();

    var host = builder.Build();

    Log.Information("PayrollLedgerSync.Worker starting.");
    await host.RunAsync();
}
catch (Exception exception)
{
    Log.Fatal(exception, "PayrollLedgerSync.Worker terminated unexpectedly.");
}
finally
{
    await Log.CloseAndFlushAsync();
}

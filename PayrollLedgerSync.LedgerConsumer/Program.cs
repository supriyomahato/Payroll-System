using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PayrollLedgerSync.LedgerConsumer.Extensions;
using PayrollLedgerSync.LedgerConsumer.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddLogging();
builder.Services.AddLedgerConsumerDependencies(builder.Configuration);
builder.Services.AddTransient<LedgerConsumerRunner>();

using var host = builder.Build();
using var scope = host.Services.CreateScope();

var runner = scope.ServiceProvider.GetRequiredService<LedgerConsumerRunner>();
await runner.RunAsync(CancellationToken.None);

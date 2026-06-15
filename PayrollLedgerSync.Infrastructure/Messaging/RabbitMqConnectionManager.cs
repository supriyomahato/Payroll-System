using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayrollLedgerSync.Application.Common.Options;
using RabbitMQ.Client;

namespace PayrollLedgerSync.Infrastructure.Messaging;

/// <summary>
/// Manages a shared RabbitMQ connection for the outbox publisher.
/// </summary>
public sealed class RabbitMqConnectionManager : IAsyncDisposable
{
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConnectionManager> _logger;
    private IConnection? _connection;

    public RabbitMqConnectionManager(IOptions<RabbitMqOptions> options, ILogger<RabbitMqConnectionManager> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true })
        {
            return _connection;
        }

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_connection is { IsOpen: true })
            {
                return _connection;
            }

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                DispatchConsumersAsync = true
            };

            _connection = await Task.Run(() => factory.CreateConnection(), cancellationToken);

            _logger.LogInformation(
                "Connected to RabbitMQ at {HostName}:{Port}, virtual host {VirtualHost}.",
                _options.HostName,
                _options.Port,
                _options.VirtualHost);

            return _connection;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public ValueTask DisposeAsync()
    {
        if (_connection is { IsOpen: true })
        {
            _connection.Close();
        }

        _connection?.Dispose();
        _connectionLock.Dispose();

        return ValueTask.CompletedTask;
    }
}

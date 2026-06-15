using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Application.Common.Options;
using PayrollLedgerSync.Domain.Entities;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;

namespace PayrollLedgerSync.Infrastructure.Messaging;

public sealed class RabbitMqOutboxMessagePublisher(
    RabbitMqConnectionManager connectionManager,
    IOptions<RabbitMqOptions> options,
    ILogger<RabbitMqOutboxMessagePublisher> logger) : IOutboxMessagePublisher
{
    private readonly AsyncRetryPolicy _publishRetryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
            onRetry: (exception, delay, retryAttempt, _) =>
            {
                logger.LogWarning(
                    exception,
                    "RabbitMQ publish retry {RetryAttempt} in {DelaySeconds}s.",
                    retryAttempt,
                    delay.TotalSeconds);
            });

    public Task PublishAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken)
    {
        return _publishRetryPolicy.ExecuteAsync(
            ct => PublishInternalAsync(outboxEvent, ct),
            cancellationToken);
    }

    private async Task PublishInternalAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken)
    {
        var rabbitOptions = options.Value;
        var connection = await connectionManager.GetConnectionAsync(cancellationToken);

        await Task.Run(() =>
        {
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: rabbitOptions.ExchangeName,
                type: rabbitOptions.ExchangeType,
                durable: true,
                autoDelete: false);

            var properties = channel.CreateBasicProperties();
            properties.ContentType = "application/json";
            properties.DeliveryMode = 2;
            properties.MessageId = outboxEvent.Id.ToString();
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            properties.Headers = new Dictionary<string, object>
            {
                ["event-type"] = outboxEvent.EventType,
                ["occurred-on-utc"] = outboxEvent.OccurredOnUtc.ToString("O")
            };

            var body = Encoding.UTF8.GetBytes(outboxEvent.Payload);
            var routingKey = ToRoutingKey(outboxEvent.EventType);

            channel.BasicPublish(
                exchange: rabbitOptions.ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body);

            logger.LogDebug(
                "Published outbox event {OutboxEventId} to exchange {Exchange} with routing key {RoutingKey}.",
                outboxEvent.Id,
                rabbitOptions.ExchangeName,
                routingKey);
        }, cancellationToken);
    }

    private static string ToRoutingKey(string eventType)
    {
        return eventType
            .Replace("DomainEvent", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace('.', '-')
            .ToLowerInvariant();
    }
}

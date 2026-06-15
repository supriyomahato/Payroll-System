using PayrollLedgerSync.Domain.Common;

namespace PayrollLedgerSync.Domain.Entities;

/// <summary>
/// Aggregate root for transactional outbox.
/// Business rule: outbox record is immutable except processing metadata.
/// </summary>
public sealed class OutboxEvent : AggregateRoot
{
    private OutboxEvent()
    {
        EventType = string.Empty;
        Payload = string.Empty;
    }

    public string EventType { get; private set; }
    public string Payload { get; private set; }
    public DateTime OccurredOnUtc { get; private set; }
    public DateTime? ProcessedOnUtc { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }

    public bool IsPublished => ProcessedOnUtc is not null;

    private OutboxEvent(Guid id, string eventType, string payload, DateTime occurredOnUtc, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new ArgumentException("Event type is required.", nameof(eventType));
        }

        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new ArgumentException("Payload is required.", nameof(payload));
        }

        Id = id;
        EventType = eventType.Trim();
        Payload = payload;
        OccurredOnUtc = occurredOnUtc;
        SetCreatedAudit(createdBy, DateTime.UtcNow);
    }

    public static OutboxEvent Create(string eventType, string payload, DateTime occurredOnUtc, string createdBy)
    {
        return new OutboxEvent(Guid.NewGuid(), eventType, payload, occurredOnUtc, createdBy);
    }

    public void MarkProcessed(string processedBy)
    {
        if (ProcessedOnUtc is not null)
        {
            return;
        }

        ProcessedOnUtc = DateTime.UtcNow;
        Error = null;
        Touch(processedBy, DateTime.UtcNow);
    }

    public void MarkFailed(string error, string processedBy)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("Error is required.", nameof(error));
        }

        Error = error.Trim();
        Touch(processedBy, DateTime.UtcNow);
    }

    /// <summary>
    /// Records a publish failure and increments retry count for outbox dispatcher backoff.
    /// </summary>
    public void RegisterPublishFailure(string error, string processedBy)
    {
        MarkFailed(error, processedBy);
        RetryCount++;
    }
}

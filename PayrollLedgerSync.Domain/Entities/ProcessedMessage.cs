using PayrollLedgerSync.Domain.Common;

namespace PayrollLedgerSync.Domain.Entities;

/// <summary>
/// Aggregate root for idempotent message processing.
/// Business rule: same message fingerprint + consumer pair must be processed only once.
/// </summary>
public sealed class ProcessedMessage : AggregateRoot
{
    private ProcessedMessage()
    {
        MessageFingerprint = string.Empty;
        Consumer = string.Empty;
    }

    public string MessageFingerprint { get; private set; }
    public string Consumer { get; private set; }
    public DateTime ProcessedOnUtc { get; private set; }

    private ProcessedMessage(Guid id, string messageFingerprint, string consumer, string createdBy)
    {
        if (string.IsNullOrWhiteSpace(messageFingerprint))
        {
            throw new ArgumentException("Message fingerprint is required.", nameof(messageFingerprint));
        }

        if (string.IsNullOrWhiteSpace(consumer))
        {
            throw new ArgumentException("Consumer is required.", nameof(consumer));
        }

        Id = id;
        MessageFingerprint = messageFingerprint.Trim();
        Consumer = consumer.Trim();
        ProcessedOnUtc = DateTime.UtcNow;
        SetCreatedAudit(createdBy, DateTime.UtcNow);
    }

    public static ProcessedMessage Create(string messageFingerprint, string consumer, string createdBy)
    {
        return new ProcessedMessage(Guid.NewGuid(), messageFingerprint, consumer, createdBy);
    }
}

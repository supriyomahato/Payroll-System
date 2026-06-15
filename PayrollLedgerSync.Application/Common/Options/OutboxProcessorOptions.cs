namespace PayrollLedgerSync.Application.Common.Options;

public sealed class OutboxProcessorOptions
{
    public const string SectionName = "OutboxProcessor";

    public int PollingIntervalSeconds { get; set; } = 5;

    public int BatchSize { get; set; } = 20;

    public int MaxRetryCount { get; set; } = 5;

    public string ProcessedBy { get; set; } = "outbox-processor";
}

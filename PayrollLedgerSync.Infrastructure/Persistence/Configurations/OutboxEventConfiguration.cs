using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollLedgerSync.Domain.Entities;

namespace PayrollLedgerSync.Infrastructure.Persistence.Configurations;

public sealed class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(EntityTypeBuilder<OutboxEvent> builder)
    {
        builder.ToTable("OutboxEvents");

        builder.HasKey(x => x.Id);
        builder.Property<byte[]>("RowVersion").IsRowVersion().IsConcurrencyToken();

        builder.Property(x => x.EventType).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Payload).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.OccurredOnUtc).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.ProcessedOnUtc).HasColumnType("datetime2");
        builder.Property(x => x.Error).HasMaxLength(2048);
        builder.Property(x => x.RetryCount).HasDefaultValue(0).IsRequired();
        builder.Ignore(x => x.IsPublished);

        builder.Property(x => x.CreatedOnUtc).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastModifiedOnUtc).HasColumnType("datetime2");
        builder.Property(x => x.LastModifiedBy).HasMaxLength(100);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.DeletedOnUtc).HasColumnType("datetime2");
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.Ignore(x => x.DomainEvents);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => new { x.ProcessedOnUtc, x.OccurredOnUtc }).HasDatabaseName("IX_OutboxEvents_ProcessingWindow");
        builder.HasIndex(x => x.EventType).HasDatabaseName("IX_OutboxEvents_EventType");
        builder.HasIndex(x => new { x.IsDeleted, x.OccurredOnUtc }).HasDatabaseName("IX_OutboxEvents_SoftDelete_OccurredOn");
    }
}

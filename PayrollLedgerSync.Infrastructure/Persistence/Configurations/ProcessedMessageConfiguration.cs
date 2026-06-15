using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollLedgerSync.Domain.Entities;

namespace PayrollLedgerSync.Infrastructure.Persistence.Configurations;

public sealed class ProcessedMessageConfiguration : IEntityTypeConfiguration<ProcessedMessage>
{
    public void Configure(EntityTypeBuilder<ProcessedMessage> builder)
    {
        builder.ToTable("ProcessedMessages");

        builder.HasKey(x => x.Id);
        builder.Property<byte[]>("RowVersion").IsRowVersion().IsConcurrencyToken();

        builder.Property(x => x.MessageFingerprint).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Consumer).HasMaxLength(150).IsRequired();
        builder.Property(x => x.ProcessedOnUtc).HasColumnType("datetime2").IsRequired();

        builder.Property(x => x.CreatedOnUtc).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastModifiedOnUtc).HasColumnType("datetime2");
        builder.Property(x => x.LastModifiedBy).HasMaxLength(100);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.DeletedOnUtc).HasColumnType("datetime2");
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.Ignore(x => x.DomainEvents);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasIndex(x => new { x.MessageFingerprint, x.Consumer })
            .IsUnique()
            .HasDatabaseName("UX_ProcessedMessages_Fingerprint_Consumer");
        builder.HasIndex(x => new { x.IsDeleted, x.ProcessedOnUtc }).HasDatabaseName("IX_ProcessedMessages_SoftDelete_ProcessedOn");
    }
}

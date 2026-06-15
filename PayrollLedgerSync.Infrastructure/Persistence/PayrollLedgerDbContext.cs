using Microsoft.EntityFrameworkCore;
using PayrollLedgerSync.Domain.Entities;
using PayrollLedgerSync.Infrastructure.Persistence.Configurations;

namespace PayrollLedgerSync.Infrastructure.Persistence;

public sealed class PayrollLedgerDbContext(DbContextOptions<PayrollLedgerDbContext> options) : DbContext(options)
{
    public DbSet<PayrollBatch> PayrollBatches => Set<PayrollBatch>();
    public DbSet<OutboxEvent> OutboxEvents => Set<OutboxEvent>();
    public DbSet<ProcessedMessage> ProcessedMessages => Set<ProcessedMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PayrollBatchConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxEventConfiguration());
        modelBuilder.ApplyConfiguration(new ProcessedMessageConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditTimestamps()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<PayrollLedgerSync.Domain.Common.AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.CreatedOnUtc == default)
                {
                    entry.Property(x => x.CreatedOnUtc).CurrentValue = utcNow;
                }

                if (string.IsNullOrWhiteSpace(entry.Entity.CreatedBy))
                {
                    entry.Property(x => x.CreatedBy).CurrentValue = "system";
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(x => x.LastModifiedOnUtc).CurrentValue = utcNow;
                if (string.IsNullOrWhiteSpace(entry.Entity.LastModifiedBy))
                {
                    entry.Property(x => x.LastModifiedBy).CurrentValue = "system";
                }
            }
        }
    }
}

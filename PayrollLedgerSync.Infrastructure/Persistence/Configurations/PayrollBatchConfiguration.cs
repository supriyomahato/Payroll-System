using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayrollLedgerSync.Domain.Entities;

namespace PayrollLedgerSync.Infrastructure.Persistence.Configurations;

public sealed class PayrollBatchConfiguration : IEntityTypeConfiguration<PayrollBatch>
{
    public void Configure(EntityTypeBuilder<PayrollBatch> builder)
    {
        builder.ToTable("PayrollBatches");

        builder.HasKey(x => x.Id);
        builder.Property<byte[]>("RowVersion").IsRowVersion().IsConcurrencyToken();

        builder.Property(x => x.Status).HasConversion<int>().IsRequired();

        builder.Property(x => x.PayrollPeriod)
            .HasConversion(
                period => period.ToString(),
                value => PayrollLedgerSync.Domain.ValueObjects.PayrollPeriod.Parse(value))
            .HasColumnName("Period")
            .HasMaxLength(7)
            .IsRequired();

        builder.Ignore(x => x.Period);
        builder.Ignore(x => x.NetAmount);
        builder.Ignore(x => x.IsSyncedToLedger);
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.CreatedOnUtc).HasColumnType("datetime2").IsRequired();
        builder.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
        builder.Property(x => x.LastModifiedOnUtc).HasColumnType("datetime2");
        builder.Property(x => x.LastModifiedBy).HasMaxLength(100);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false).IsRequired();
        builder.Property(x => x.DeletedOnUtc).HasColumnType("datetime2");
        builder.Property(x => x.DeletedBy).HasMaxLength(100);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.OwnsMany(x => x.EmployeePayrolls, employee =>
        {
            employee.ToTable("EmployeePayrolls");
            employee.WithOwner().HasForeignKey("PayrollBatchId");
            employee.HasKey(x => x.Id);

            employee.Property<Guid>("PayrollBatchId").IsRequired();
            employee.Property(x => x.Id).ValueGeneratedNever();

            employee.Property(x => x.EmployeeCode)
                .HasConversion(code => code.Value, value => PayrollLedgerSync.Domain.ValueObjects.EmployeeCode.Of(value))
                .HasColumnName("EmployeeCode")
                .HasMaxLength(32)
                .IsRequired();

            employee.OwnsOne(x => x.GrossPay, money =>
            {
                money.Property(x => x.Amount).HasColumnName("GrossAmount").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(x => x.Currency).HasColumnName("GrossCurrency").HasMaxLength(3).IsRequired();
            });

            employee.OwnsOne(x => x.Deductions, money =>
            {
                money.Property(x => x.Amount).HasColumnName("DeductionAmount").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(x => x.Currency).HasColumnName("DeductionCurrency").HasMaxLength(3).IsRequired();
            });

            employee.OwnsOne(x => x.NetPay, money =>
            {
                money.Property(x => x.Amount).HasColumnName("NetAmount").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(x => x.Currency).HasColumnName("NetCurrency").HasMaxLength(3).IsRequired();
            });

            employee.HasIndex("PayrollBatchId", "EmployeeCode").HasDatabaseName("IX_EmployeePayrolls_Batch_EmployeeCode");
        });

        builder.OwnsMany(x => x.LedgerEntries, ledger =>
        {
            ledger.ToTable("LedgerEntries");
            ledger.WithOwner().HasForeignKey("PayrollBatchId");
            ledger.HasKey(x => x.Id);

            ledger.Property<Guid>("PayrollBatchId").IsRequired();
            ledger.Property(x => x.Id).ValueGeneratedNever();
            ledger.Property(x => x.AccountCode).HasMaxLength(32).IsRequired();
            ledger.Property(x => x.Description).HasMaxLength(256).IsRequired();

            ledger.OwnsOne(x => x.Debit, money =>
            {
                money.Property(x => x.Amount).HasColumnName("DebitAmount").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(x => x.Currency).HasColumnName("DebitCurrency").HasMaxLength(3).IsRequired();
            });

            ledger.OwnsOne(x => x.Credit, money =>
            {
                money.Property(x => x.Amount).HasColumnName("CreditAmount").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(x => x.Currency).HasColumnName("CreditCurrency").HasMaxLength(3).IsRequired();
            });

            ledger.HasIndex("PayrollBatchId", "AccountCode").HasDatabaseName("IX_LedgerEntries_Batch_AccountCode");
        });

        builder.Metadata.FindNavigation(nameof(PayrollBatch.EmployeePayrolls))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(PayrollBatch.LedgerEntries))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.PayrollPeriod).HasDatabaseName("IX_PayrollBatches_Period");
        builder.HasIndex(x => x.Status).HasDatabaseName("IX_PayrollBatches_Status");
        builder.HasIndex(x => new { x.IsDeleted, x.CreatedOnUtc }).HasDatabaseName("IX_PayrollBatches_SoftDelete_CreatedOn");
    }
}

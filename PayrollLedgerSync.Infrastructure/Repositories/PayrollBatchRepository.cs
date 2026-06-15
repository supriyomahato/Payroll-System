using Microsoft.EntityFrameworkCore;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Domain.Entities;
using PayrollLedgerSync.Domain.ValueObjects;
using PayrollLedgerSync.Infrastructure.Persistence;

namespace PayrollLedgerSync.Infrastructure.Repositories;

public sealed class PayrollBatchRepository(PayrollLedgerDbContext dbContext) : IPayrollBatchRepository
{
    public async Task AddAsync(PayrollBatch payrollBatch, CancellationToken cancellationToken)
    {
        await dbContext.PayrollBatches.AddAsync(payrollBatch, cancellationToken);
    }

    public Task<PayrollBatch?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.PayrollBatches
            .Include(x => x.EmployeePayrolls)
            .Include(x => x.LedgerEntries)
            .FirstOrDefaultAsync(batch => batch.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<PayrollBatch>> GetPendingSyncAsync(CancellationToken cancellationToken)
    {
        var pending = await dbContext.PayrollBatches
            .Include(x => x.EmployeePayrolls)
            .Include(x => x.LedgerEntries)
            .Where(batch => batch.Status != PayrollBatchStatus.Synced)
            .ToListAsync(cancellationToken);

        return pending;
    }

    public Task<bool> ExistsForPeriodAsync(string period, CancellationToken cancellationToken)
    {
        var payrollPeriod = PayrollPeriod.Parse(period);
        return dbContext.PayrollBatches.AnyAsync(
            batch => batch.PayrollPeriod == payrollPeriod,
            cancellationToken);
    }
}

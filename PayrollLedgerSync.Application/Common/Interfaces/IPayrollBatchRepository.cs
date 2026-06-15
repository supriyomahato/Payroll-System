using PayrollLedgerSync.Domain.Entities;

namespace PayrollLedgerSync.Application.Common.Interfaces;

public interface IPayrollBatchRepository
{
    Task<PayrollBatch?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(PayrollBatch payrollBatch, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PayrollBatch>> GetPendingSyncAsync(CancellationToken cancellationToken);
    Task<bool> ExistsForPeriodAsync(string period, CancellationToken cancellationToken);
}

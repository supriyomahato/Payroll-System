using MediatR;
using PayrollLedgerSync.Application.Common.Interfaces;
using PayrollLedgerSync.Domain.Entities;

namespace PayrollLedgerSync.Application.LedgerSync.Commands;

public sealed record SyncPayrollBatchCommand(Guid PayrollBatchId) : IRequest<bool>;

public sealed class SyncPayrollBatchCommandHandler(IPayrollBatchRepository payrollBatchRepository, IUnitOfWork unitOfWork, IDomainEventDispatcher domainEventDispatcher)
    : IRequestHandler<SyncPayrollBatchCommand, bool>
{
    public async Task<bool> Handle(SyncPayrollBatchCommand request, CancellationToken cancellationToken)
    {
        var batch = await payrollBatchRepository.GetByIdAsync(request.PayrollBatchId, cancellationToken);
        if (batch is null)
        {
            return false;
        }

        batch.MarkAsSynced();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var events = batch.DequeueDomainEvents();
        await domainEventDispatcher.DispatchAsync(events, cancellationToken);

        return true;
    }
}

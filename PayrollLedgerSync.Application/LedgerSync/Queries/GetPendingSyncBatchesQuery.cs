using MediatR;
using PayrollLedgerSync.Application.Common.Interfaces;

namespace PayrollLedgerSync.Application.LedgerSync.Queries;

public sealed record GetPendingSyncBatchesQuery : IRequest<IReadOnlyCollection<PendingSyncBatchDto>>;

public sealed record PendingSyncBatchDto(Guid PayrollBatchId, string Period, decimal NetAmount);

public sealed class GetPendingSyncBatchesQueryHandler(IPayrollBatchRepository payrollBatchRepository)
    : IRequestHandler<GetPendingSyncBatchesQuery, IReadOnlyCollection<PendingSyncBatchDto>>
{
    public async Task<IReadOnlyCollection<PendingSyncBatchDto>> Handle(GetPendingSyncBatchesQuery request, CancellationToken cancellationToken)
    {
        var batches = await payrollBatchRepository.GetPendingSyncAsync(cancellationToken);
        return batches
            .Select(batch => new PendingSyncBatchDto(batch.Id, batch.Period, batch.NetAmount))
            .ToArray();
    }
}

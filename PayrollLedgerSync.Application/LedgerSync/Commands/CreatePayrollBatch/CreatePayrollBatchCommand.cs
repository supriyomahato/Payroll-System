using MediatR;

namespace PayrollLedgerSync.Application.LedgerSync.Commands.CreatePayrollBatch;

public sealed record CreatePayrollBatchCommand(CreatePayrollBatchRequest Request)
    : IRequest<CreatePayrollBatchResponse>;

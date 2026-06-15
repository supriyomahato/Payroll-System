using FluentValidation;
using MediatR;
using PayrollLedgerSync.Application.LedgerSync.Commands;
using PayrollLedgerSync.Application.LedgerSync.Commands.CreatePayrollBatch;
using PayrollLedgerSync.Application.LedgerSync.Queries;

namespace PayrollLedgerSync.Api.Endpoints;

public static class LedgerSyncEndpoints
{
    public static IEndpointRouteBuilder MapLedgerSyncEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ledger-sync");

        group.MapPost("/", async (CreatePayrollBatchRequest request, ISender sender, CancellationToken cancellationToken) =>
        {
            try
            {
                var response = await sender.Send(new CreatePayrollBatchCommand(request), cancellationToken);
                return Results.Created($"/api/ledger-sync/{response.PayrollBatchId}", response);
            }
            catch (ValidationException validationException)
            {
                return Results.ValidationProblem(validationException.Errors
                    .GroupBy(error => error.PropertyName)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(error => error.ErrorMessage).ToArray()));
            }
            catch (InvalidOperationException invalidOperationException)
            {
                return Results.Conflict(new { error = invalidOperationException.Message });
            }
        });

        group.MapGet("/pending", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetPendingSyncBatchesQuery(), cancellationToken);
            return Results.Ok(result);
        });

        group.MapPost("/{batchId:guid}", async (Guid batchId, ISender sender, CancellationToken cancellationToken) =>
        {
            var synced = await sender.Send(new SyncPayrollBatchCommand(batchId), cancellationToken);
            return synced ? Results.Accepted($"/api/ledger-sync/{batchId}") : Results.NotFound();
        });

        return app;
    }
}

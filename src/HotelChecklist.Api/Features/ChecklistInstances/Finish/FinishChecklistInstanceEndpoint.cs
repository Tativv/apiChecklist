using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.Finish;

public static class FinishChecklistInstanceEndpoint
{
    public static void MapFinishChecklistInstance(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/finish", async (
                Guid id,
                ICommandHandler<FinishChecklistInstanceCommand, FinishChecklistInstanceResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new FinishChecklistInstanceCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("FinishChecklistInstance")
            .Produces<FinishChecklistInstanceResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}

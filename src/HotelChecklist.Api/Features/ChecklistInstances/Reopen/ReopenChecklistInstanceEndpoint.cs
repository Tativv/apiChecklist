using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Api.Common.Validation;

namespace HotelChecklist.Api.Features.ChecklistInstances.Reopen;

public static class ReopenChecklistInstanceEndpoint
{
    public static void MapReopenChecklistInstance(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/reopen", async (
                Guid id,
                ReopenChecklistInstanceRequest request,
                ICommandHandler<ReopenChecklistInstanceCommand, ReopenChecklistInstanceResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(request.ToCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .AddEndpointFilter<ValidationFilter<ReopenChecklistInstanceRequest>>()
            .RequireAuthorization(Policies.ManagerOrAbove)
            .WithName("ReopenChecklistInstance")
            .Produces<ReopenChecklistInstanceResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

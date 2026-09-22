using System.Security.Claims;
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
                ClaimsPrincipal user,
                ICommandHandler<FinishChecklistInstanceCommand, FinishChecklistInstanceResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new FinishChecklistInstanceCommand(id, user.GetUserId(), user.IsSupervisorOrAbove());
                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("FinishChecklistInstance")
            .Produces<FinishChecklistInstanceResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}

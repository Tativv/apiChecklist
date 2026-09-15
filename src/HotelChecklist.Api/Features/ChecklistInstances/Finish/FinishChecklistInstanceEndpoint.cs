using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;
using HotelChecklist.Domain.Enums;

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
                var isSupervisorOrAbove = user.IsInRole(UserRole.Admin.ToString())
                    || user.IsInRole(UserRole.Manager.ToString())
                    || user.IsInRole(UserRole.Supervisor.ToString());

                var command = new FinishChecklistInstanceCommand(id, user.GetUserId(), isSupervisorOrAbove);
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

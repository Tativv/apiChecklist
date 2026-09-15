using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.Approve;

public static class ApproveChecklistInstanceEndpoint
{
    public static void MapApproveChecklistInstance(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/approve", async (
                Guid id,
                ClaimsPrincipal user,
                ICommandHandler<ApproveChecklistInstanceCommand, ApproveChecklistInstanceResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new ApproveChecklistInstanceCommand(id, user.GetUserId()), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("ApproveChecklistInstance")
            .Produces<ApproveChecklistInstanceResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

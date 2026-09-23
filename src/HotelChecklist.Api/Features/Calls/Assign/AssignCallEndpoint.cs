using System.Security.Claims;
using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Calls.Assign;

public static class AssignCallEndpoint
{
    public static void MapAssignCall(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/assign", async (
                Guid id,
                AssignCallRequest request,
                ClaimsPrincipal user,
                ICommandHandler<AssignCallCommand, AssignCallResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var command = request.ToCommand(id, user.GetUserId(), user.IsExactlySupervisor(), user.IsExactlyColaborador());
                var result = await handler.Handle(command, cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("AssignCall")
            .Produces<AssignCallResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }
}

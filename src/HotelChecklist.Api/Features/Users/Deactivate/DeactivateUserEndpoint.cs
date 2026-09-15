using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Users.Deactivate;

public static class DeactivateUserEndpoint
{
    public static void MapDeactivateUser(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/deactivate", async (
                Guid id,
                ICommandHandler<DeactivateUserCommand, Unit> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new DeactivateUserCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AdminOnly)
            .WithName("DeactivateUser")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

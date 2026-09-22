using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.Delete;

public static class DeleteChecklistInstanceEndpoint
{
    public static void MapDeleteChecklistInstance(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", async (
                Guid id,
                ICommandHandler<DeleteChecklistInstanceCommand, Unit> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new DeleteChecklistInstanceCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.ManagerOrAbove)
            .WithName("DeleteChecklistInstance")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}

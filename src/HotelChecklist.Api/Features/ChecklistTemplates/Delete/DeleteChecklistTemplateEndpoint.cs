using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Delete;

public static class DeleteChecklistTemplateEndpoint
{
    public static void MapDeleteChecklistTemplate(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", async (
                Guid id,
                ICommandHandler<DeleteChecklistTemplateCommand, Unit> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new DeleteChecklistTemplateCommand(id), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("DeleteChecklistTemplate")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}

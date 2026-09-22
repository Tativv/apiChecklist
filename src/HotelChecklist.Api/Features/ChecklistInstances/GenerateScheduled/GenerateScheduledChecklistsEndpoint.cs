using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.GenerateScheduled;

public static class GenerateScheduledChecklistsEndpoint
{
    public static void MapGenerateScheduledChecklists(this RouteGroupBuilder group)
    {
        group.MapPost("/generate-scheduled", async (
                DateOnly? date,
                ICommandHandler<GenerateScheduledChecklistsCommand, GenerateScheduledChecklistsResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GenerateScheduledChecklistsCommand(date), cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .RequireAuthorization(Policies.ManagerOrAbove)
            .WithName("GenerateScheduledChecklists")
            .Produces<GenerateScheduledChecklistsResponse>(StatusCodes.Status201Created);
    }
}

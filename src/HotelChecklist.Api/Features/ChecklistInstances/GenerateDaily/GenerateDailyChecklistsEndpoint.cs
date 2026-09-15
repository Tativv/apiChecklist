using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.GenerateDaily;

public static class GenerateDailyChecklistsEndpoint
{
    public static void MapGenerateDailyChecklists(this RouteGroupBuilder group)
    {
        group.MapPost("/generate-daily", async (
                DateOnly? date,
                ICommandHandler<GenerateDailyChecklistsCommand, GenerateDailyChecklistsResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GenerateDailyChecklistsCommand(date), cancellationToken);
                return result.ToHttpResult(StatusCodes.Status201Created);
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("GenerateDailyChecklists")
            .Produces<GenerateDailyChecklistsResponse>(StatusCodes.Status201Created);
    }
}

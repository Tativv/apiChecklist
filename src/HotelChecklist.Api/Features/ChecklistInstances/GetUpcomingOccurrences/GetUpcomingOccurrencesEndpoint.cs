using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.ChecklistInstances.GetUpcomingOccurrences;

public static class GetUpcomingOccurrencesEndpoint
{
    public static void MapGetUpcomingOccurrences(this RouteGroupBuilder group)
    {
        group.MapGet("/upcoming", async (
                DateOnly from,
                DateOnly to,
                IQueryHandler<GetUpcomingOccurrencesQuery, IReadOnlyList<UpcomingOccurrenceItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new GetUpcomingOccurrencesQuery(from, to), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.AnyRole)
            .WithName("GetUpcomingOccurrences")
            .Produces<IReadOnlyList<UpcomingOccurrenceItem>>()
            .ProducesValidationProblem();
    }
}

using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Reports.ByArea;

public static class ByAreaReportEndpoint
{
    public static void MapByAreaReport(this RouteGroupBuilder group)
    {
        group.MapGet("/by-area", async (
                DateOnly? fromDate,
                DateOnly? toDate,
                IQueryHandler<ByAreaReportQuery, IReadOnlyList<ByAreaReportResponseItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var effectiveFrom = fromDate ?? today.AddDays(-30);
                var effectiveTo = toDate ?? today;

                var result = await handler.Handle(new ByAreaReportQuery(effectiveFrom, effectiveTo), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("ByAreaReport")
            .Produces<IReadOnlyList<ByAreaReportResponseItem>>();
    }
}

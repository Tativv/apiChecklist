using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Reports.ByDate;

public static class ByDateReportEndpoint
{
    public static void MapByDateReport(this RouteGroupBuilder group)
    {
        group.MapGet("/by-date", async (
                DateOnly? fromDate,
                DateOnly? toDate,
                IQueryHandler<ByDateReportQuery, IReadOnlyList<ByDateReportResponseItem>> handler,
                CancellationToken cancellationToken) =>
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var effectiveFrom = fromDate ?? today.AddDays(-30);
                var effectiveTo = toDate ?? today;

                var result = await handler.Handle(new ByDateReportQuery(effectiveFrom, effectiveTo), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("ByDateReport")
            .Produces<IReadOnlyList<ByDateReportResponseItem>>();
    }
}

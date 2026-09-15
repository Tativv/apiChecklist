using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Errors;

namespace HotelChecklist.Api.Features.Reports.Dashboard;

public static class DashboardEndpoint
{
    public static void MapDashboard(this RouteGroupBuilder group)
    {
        group.MapGet("/dashboard", async (
                DateOnly? fromDate,
                DateOnly? toDate,
                IQueryHandler<DashboardQuery, DashboardResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.Handle(new DashboardQuery(fromDate, toDate), cancellationToken);
                return result.ToHttpResult();
            })
            .RequireAuthorization(Policies.SupervisorOrAbove)
            .WithName("Dashboard")
            .Produces<DashboardResponse>();
    }
}

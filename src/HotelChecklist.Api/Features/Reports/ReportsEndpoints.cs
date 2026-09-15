using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Features.Reports.ByArea;
using HotelChecklist.Api.Features.Reports.ByDate;
using HotelChecklist.Api.Features.Reports.Dashboard;

namespace HotelChecklist.Api.Features.Reports;

public static class ReportsEndpoints
{
    public static IServiceCollection AddReportsFeature(this IServiceCollection services)
    {
        services.AddScoped<IQueryHandler<DashboardQuery, DashboardResponse>, DashboardHandler>();
        services.AddScoped<IQueryHandler<ByDateReportQuery, IReadOnlyList<ByDateReportResponseItem>>, ByDateReportHandler>();
        services.AddScoped<IQueryHandler<ByAreaReportQuery, IReadOnlyList<ByAreaReportResponseItem>>, ByAreaReportHandler>();

        return services;
    }

    public static void MapReportsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reports").WithTags("Reports");

        group.MapDashboard();
        group.MapByDateReport();
        group.MapByAreaReport();
    }
}

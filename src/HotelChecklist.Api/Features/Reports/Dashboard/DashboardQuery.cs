using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Reports.Dashboard;

public sealed record DashboardQuery(DateOnly? FromDate, DateOnly? ToDate, DateOnly? Today) : IQuery<DashboardResponse>;

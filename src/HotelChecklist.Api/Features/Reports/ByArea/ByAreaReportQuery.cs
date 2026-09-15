using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Reports.ByArea;

public sealed record ByAreaReportQuery(DateOnly FromDate, DateOnly ToDate) : IQuery<IReadOnlyList<ByAreaReportResponseItem>>;

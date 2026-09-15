using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Reports.ByDate;

public sealed record ByDateReportQuery(DateOnly FromDate, DateOnly ToDate) : IQuery<IReadOnlyList<ByDateReportResponseItem>>;

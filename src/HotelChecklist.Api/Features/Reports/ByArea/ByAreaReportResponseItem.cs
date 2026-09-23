namespace HotelChecklist.Api.Features.Reports.ByArea;

public sealed record ByAreaReportResponseItem(
    Guid AreaId,
    string AreaName,
    int Total,
    int Pending,
    int Approved,
    int InProgress,
    int Completed,
    int Reviewed,
    double? AverageDurationSeconds);

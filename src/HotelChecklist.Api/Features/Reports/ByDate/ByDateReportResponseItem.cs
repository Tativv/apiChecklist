namespace HotelChecklist.Api.Features.Reports.ByDate;

public sealed record ByDateReportResponseItem(
    DateOnly Date,
    int Total,
    int Pending,
    int Approved,
    int InProgress,
    int Completed,
    int Reviewed,
    double? AverageDurationSeconds);

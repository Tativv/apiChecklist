namespace HotelChecklist.Api.Features.Reports.ByDate;

public sealed record ByDateReportResponseItem(
    DateOnly Date,
    int Total,
    int Pending,
    int InProgress,
    int Completed,
    int Approved,
    double? AverageDurationSeconds);

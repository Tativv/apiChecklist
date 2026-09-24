namespace HotelChecklist.Api.Features.Reports.ByArea;

public sealed record ByAreaReportResponseItem(
    Guid AreaId,
    string AreaName,
    int Total,
    int Pending,
    int InProgress,
    int Completed,
    double? AverageDurationSeconds,
    int TasksTotal,
    int TasksPending,
    int TasksInProgress,
    int TasksCompleted,
    int TasksReviewed,
    double? AverageTaskDurationSeconds);

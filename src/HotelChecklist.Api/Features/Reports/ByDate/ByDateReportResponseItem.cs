namespace HotelChecklist.Api.Features.Reports.ByDate;

public sealed record ByDateReportResponseItem(
    DateOnly Date,
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

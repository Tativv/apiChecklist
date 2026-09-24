namespace HotelChecklist.Api.Features.Reports.Dashboard;

public sealed record DashboardResponse(
    int Total,
    int Pending,
    int InProgress,
    int Completed,
    int Overdue,
    double? AverageDurationSeconds,
    double CompletionRatePercent,
    int TasksTotal,
    int TasksPending,
    int TasksInProgress,
    int TasksCompleted,
    int TasksReviewed,
    int TasksOverdue,
    double? AverageTaskDurationSeconds);

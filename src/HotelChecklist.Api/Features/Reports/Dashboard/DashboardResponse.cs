namespace HotelChecklist.Api.Features.Reports.Dashboard;

public sealed record DashboardResponse(
    int Total,
    int Pending,
    int InProgress,
    int Completed,
    int Overdue,
    double? AverageDurationSeconds,
    double CompletionRatePercent);

namespace HotelChecklist.Api.Features.Reports.Dashboard;

public sealed record DashboardResponse(
    int Total,
    int Pending,
    int Approved,
    int InProgress,
    int Completed,
    int Reviewed,
    int Overdue,
    double? AverageDurationSeconds,
    double CompletionRatePercent);

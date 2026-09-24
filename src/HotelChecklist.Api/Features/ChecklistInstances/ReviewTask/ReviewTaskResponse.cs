namespace HotelChecklist.Api.Features.ChecklistInstances.ReviewTask;

public sealed record ReviewTaskResponse(Guid Id, string Status, Guid? ApprovedByUserId, DateTimeOffset? ApprovedAt);

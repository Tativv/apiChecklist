namespace HotelChecklist.Api.Features.ChecklistInstances.Approve;

public sealed record ApproveChecklistInstanceResponse(Guid Id, string Status, Guid ApprovedByUserId, DateTimeOffset? ApprovedAt);

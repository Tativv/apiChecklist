namespace HotelChecklist.Api.Features.ChecklistInstances.AssignTask;

public sealed record AssignTaskResponse(Guid Id, Guid? AssignedUserId, Guid? CreatedByUserId);

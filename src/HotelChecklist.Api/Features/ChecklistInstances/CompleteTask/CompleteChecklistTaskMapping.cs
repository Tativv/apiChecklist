namespace HotelChecklist.Api.Features.ChecklistInstances.CompleteTask;

public static class CompleteChecklistTaskMapping
{
    public static CompleteChecklistTaskCommand ToCommand(
        this CompleteChecklistTaskRequest request, Guid instanceId, Guid taskExecutionId, Guid actingUserId, bool actingUserIsSupervisorOrAbove) =>
        new(instanceId, taskExecutionId, request.Comment, actingUserId, actingUserIsSupervisorOrAbove);
}

namespace HotelChecklist.Api.Features.ChecklistInstances.AssignTask;

public static class AssignTaskMapping
{
    public static AssignTaskCommand ToCommand(
        this AssignTaskRequest request, Guid instanceId, Guid taskExecutionId, Guid actingUserId, bool actingUserIsExactlySupervisor) =>
        new(instanceId, taskExecutionId, request.UserId, request.EstimatedDurationMinutes, actingUserId, actingUserIsExactlySupervisor);
}

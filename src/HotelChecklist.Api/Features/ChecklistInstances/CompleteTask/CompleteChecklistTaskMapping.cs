namespace HotelChecklist.Api.Features.ChecklistInstances.CompleteTask;

public static class CompleteChecklistTaskMapping
{
    public static CompleteChecklistTaskCommand ToCommand(this CompleteChecklistTaskRequest request, Guid instanceId, Guid taskExecutionId) =>
        new(instanceId, taskExecutionId, request.Completed, request.Comment);
}

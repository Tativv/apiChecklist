namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public static class AddTaskCommentMapping
{
    public static AddTaskCommentCommand ToCommand(this AddTaskCommentRequest request, Guid instanceId, Guid taskExecutionId, Guid actingUserId) =>
        new(instanceId, taskExecutionId, request.Text, actingUserId);
}

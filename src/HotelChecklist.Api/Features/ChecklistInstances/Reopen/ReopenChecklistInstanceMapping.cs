namespace HotelChecklist.Api.Features.ChecklistInstances.Reopen;

public static class ReopenChecklistInstanceMapping
{
    public static ReopenChecklistInstanceCommand ToCommand(this ReopenChecklistInstanceRequest request, Guid id) => new(id, request.Reason);
}

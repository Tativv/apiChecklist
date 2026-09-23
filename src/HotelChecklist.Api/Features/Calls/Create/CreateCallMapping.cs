namespace HotelChecklist.Api.Features.Calls.Create;

public static class CreateCallMapping
{
    public static CreateCallCommand ToCommand(this CreateCallRequest request, Guid createdByUserId) =>
        new(request.AreaId, request.Subject, request.Description, request.Priority, createdByUserId);
}

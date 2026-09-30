namespace HotelChecklist.Api.Features.Calls.Update;

public static class UpdateCallMapping
{
    public static UpdateCallCommand ToCommand(this UpdateCallRequest request, Guid id) =>
        new(id, request.AreaId, request.Subject, request.Description, request.Priority);
}

namespace HotelChecklist.Api.Features.Calls.Assign;

public static class AssignCallMapping
{
    public static AssignCallCommand ToCommand(
        this AssignCallRequest request, Guid callId, Guid actingUserId, bool actingUserIsExactlySupervisor, bool actingUserIsExactlyColaborador) =>
        new(callId, request.UserId, actingUserId, actingUserIsExactlySupervisor, actingUserIsExactlyColaborador);
}

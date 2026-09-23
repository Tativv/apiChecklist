namespace HotelChecklist.Api.Features.Calls.Assign;

public sealed record AssignCallResponse(Guid Id, Guid? AssignedUserId, string? AssignedUserName);

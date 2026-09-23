using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Calls.Assign;

public sealed record AssignCallCommand(
    Guid CallId,
    Guid? UserId,
    Guid ActingUserId,
    bool ActingUserIsExactlySupervisor,
    bool ActingUserIsExactlyColaborador) : ICommand<AssignCallResponse>;

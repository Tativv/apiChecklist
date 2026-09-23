using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Calls.Start;

public sealed record StartCallCommand(Guid Id, Guid ActingUserId, bool ActingUserIsSupervisorOrAbove) : ICommand<StartCallResponse>;

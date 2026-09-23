using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Calls.Finish;

public sealed record FinishCallCommand(Guid Id, Guid ActingUserId, bool ActingUserIsSupervisorOrAbove) : ICommand<FinishCallResponse>;

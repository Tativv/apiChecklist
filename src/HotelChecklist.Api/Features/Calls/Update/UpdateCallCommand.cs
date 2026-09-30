using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Calls.Update;

public sealed record UpdateCallCommand(
    Guid Id,
    Guid AreaId,
    string Subject,
    string? Description,
    string Priority) : ICommand<UpdateCallResponse>;

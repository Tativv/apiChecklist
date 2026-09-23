using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Calls.Create;

public sealed record CreateCallCommand(
    Guid AreaId,
    string Subject,
    string? Description,
    string Priority,
    Guid CreatedByUserId) : ICommand<CreateCallResponse>;

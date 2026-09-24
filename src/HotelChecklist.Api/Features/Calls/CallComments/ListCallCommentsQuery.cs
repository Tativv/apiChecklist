using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Calls.CallComments;

public sealed record ListCallCommentsQuery(Guid CallId) : IQuery<IReadOnlyList<CallCommentResponseItem>>;

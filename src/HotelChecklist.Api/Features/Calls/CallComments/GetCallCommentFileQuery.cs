using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Calls.CallComments;

public sealed record GetCallCommentFileQuery(Guid CommentId) : IQuery<GetCallCommentFileResponse>;

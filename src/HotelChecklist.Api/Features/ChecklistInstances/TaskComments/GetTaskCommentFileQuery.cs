using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.TaskComments;

public sealed record GetTaskCommentFileQuery(Guid CommentId) : IQuery<GetTaskCommentFileResponse>;

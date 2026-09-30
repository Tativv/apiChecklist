using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;

public sealed record GetServiceOrderCommentFileQuery(Guid CommentId) : IQuery<GetServiceOrderCommentFileResponse>;

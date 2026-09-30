using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;

public sealed record ListServiceOrderCommentsQuery(Guid ServiceOrderId) : IQuery<IReadOnlyList<ServiceOrderCommentResponseItem>>;

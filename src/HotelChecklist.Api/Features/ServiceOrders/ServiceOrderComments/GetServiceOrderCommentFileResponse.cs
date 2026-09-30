namespace HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;

public sealed record GetServiceOrderCommentFileResponse(Stream Content, string ContentType, string FileName);

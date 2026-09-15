using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Areas.Update;

public sealed record UpdateAreaCommand(Guid Id, string Name) : ICommand<UpdateAreaResponse>;

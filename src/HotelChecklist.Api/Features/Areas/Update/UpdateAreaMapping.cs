using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.Areas.Update;

public static class UpdateAreaMapping
{
    public static UpdateAreaCommand ToCommand(this UpdateAreaRequest request, Guid id) => new(id, request.Name);

    public static UpdateAreaResponse ToResponse(this Area area) => new(area.Id, area.Name);
}

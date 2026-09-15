using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.Areas.GetById;

public static class GetAreaByIdMapping
{
    public static GetAreaByIdResponse ToResponse(this Area area) => new(area.Id, area.Name);
}

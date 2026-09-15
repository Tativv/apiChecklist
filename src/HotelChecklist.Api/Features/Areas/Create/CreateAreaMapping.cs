using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.Areas.Create;

public static class CreateAreaMapping
{
    public static CreateAreaCommand ToCommand(this CreateAreaRequest request) => new(request.Name);

    public static CreateAreaResponse ToResponse(this Area area) => new(area.Id, area.Name);
}

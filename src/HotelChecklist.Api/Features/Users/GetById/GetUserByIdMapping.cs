using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.Users.GetById;

public static class GetUserByIdMapping
{
    public static GetUserByIdResponse ToResponse(this User user) =>
        new(user.Id, user.Name, user.Email, user.Role.ToString(), user.Active, user.UserAreas.Select(ua => ua.AreaId).ToList());
}

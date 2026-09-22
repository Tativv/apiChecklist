using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.Users.Create;

public static class CreateUserMapping
{
    public static CreateUserCommand ToCommand(this CreateUserRequest request) =>
        new(request.Name, request.Email, request.Password, request.Role, request.AreaIds ?? []);

    public static CreateUserResponse ToResponse(this User user) =>
        new(user.Id, user.Name, user.Email, user.Role.ToString(), user.Active, user.UserAreas.Select(ua => ua.AreaId).ToList());
}

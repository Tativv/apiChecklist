using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.Users.Create;

public static class CreateUserMapping
{
    public static CreateUserCommand ToCommand(this CreateUserRequest request) =>
        new(request.Name, request.Email, request.Password, request.Role);

    public static CreateUserResponse ToResponse(this User user) =>
        new(user.Id, user.Name, user.Email, user.Role.ToString(), user.Active);
}

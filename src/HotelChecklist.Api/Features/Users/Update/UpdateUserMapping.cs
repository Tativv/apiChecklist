using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.Users.Update;

public static class UpdateUserMapping
{
    public static UpdateUserCommand ToCommand(this UpdateUserRequest request, Guid id) => new(id, request.Name, request.Role);

    public static UpdateUserResponse ToResponse(this User user) =>
        new(user.Id, user.Name, user.Email, user.Role.ToString(), user.Active);
}

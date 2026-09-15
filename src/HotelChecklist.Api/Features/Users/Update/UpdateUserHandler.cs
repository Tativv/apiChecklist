using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.Users.Update;

public sealed class UpdateUserHandler(AppDbContext db) : ICommandHandler<UpdateUserCommand, UpdateUserResponse>
{
    public async Task<Result<UpdateUserResponse>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await db.Users.FindAsync([command.Id], cancellationToken);

        if (user is null)
            return Result.Failure<UpdateUserResponse>(Error.NotFound("Users.NotFound", "Usuario no encontrado."));

        user.Name = command.Name;
        user.Role = Enum.Parse<UserRole>(command.Role, ignoreCase: true);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(user.ToResponse());
    }
}

using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;

namespace HotelChecklist.Api.Features.Users.Deactivate;

public sealed class DeactivateUserHandler(AppDbContext db) : ICommandHandler<DeactivateUserCommand, Unit>
{
    public async Task<Result<Unit>> Handle(DeactivateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await db.Users.FindAsync([command.Id], cancellationToken);

        if (user is null)
            return Result.Failure<Unit>(Error.NotFound("Users.NotFound", "Usuario no encontrado."));

        user.Active = false;
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}

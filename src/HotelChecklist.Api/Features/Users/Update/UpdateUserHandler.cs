using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Users.Update;

public sealed class UpdateUserHandler(AppDbContext db) : ICommandHandler<UpdateUserCommand, UpdateUserResponse>
{
    public async Task<Result<UpdateUserResponse>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await db.Users.Include(u => u.UserAreas).FirstOrDefaultAsync(u => u.Id == command.Id, cancellationToken);

        if (user is null)
            return Result.Failure<UpdateUserResponse>(Error.NotFound("Users.NotFound", "Usuario no encontrado."));

        var areaIds = command.AreaIds.Distinct().ToList();
        var validAreaCount = await db.Areas.CountAsync(a => areaIds.Contains(a.Id), cancellationToken);

        if (validAreaCount != areaIds.Count)
            return Result.Failure<UpdateUserResponse>(Error.NotFound("Areas.NotFound", "Una o más áreas no existen."));

        user.Name = command.Name;
        user.Role = Enum.Parse<UserRole>(command.Role, ignoreCase: true);

        var currentAreaIds = user.UserAreas.Select(ua => ua.AreaId).ToHashSet();
        var requestedSet = areaIds.ToHashSet();

        var toRemove = user.UserAreas.Where(ua => !requestedSet.Contains(ua.AreaId)).ToList();
        var toAdd = areaIds.Where(id => !currentAreaIds.Contains(id)).ToList();

        db.UserAreas.RemoveRange(toRemove);
        db.UserAreas.AddRange(toAdd.Select(areaId => new UserArea
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            AreaId = areaId
        }));

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(user.ToResponse());
    }
}

using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using HotelChecklist.Domain.Entities;
using HotelChecklist.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.Users.Create;

public sealed class CreateUserHandler(AppDbContext db, IPasswordHasher passwordHasher) : ICommandHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var emailInUse = await db.Users.AnyAsync(u => u.Email == command.Email, cancellationToken);

        if (emailInUse)
            return Result.Failure<CreateUserResponse>(Error.Conflict("Users.EmailInUse", "Ya existe un usuario con ese email."));

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Role = Enum.Parse<UserRole>(command.Role, ignoreCase: true),
            PasswordHash = passwordHasher.Hash(command.Password),
            Active = true
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(user.ToResponse());
    }
}

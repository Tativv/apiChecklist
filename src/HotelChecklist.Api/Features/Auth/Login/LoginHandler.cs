using HotelChecklist.Api.Common.Auth;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace HotelChecklist.Api.Features.Auth.Login;

public sealed class LoginHandler(
    AppDbContext db,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator tokenGenerator,
    IOptions<JwtOptions> jwtOptions) : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Email == command.Email && u.Active, cancellationToken);

        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
            return Result.Failure<LoginResponse>(Error.Unauthorized("Auth.InvalidCredentials", "Email o contraseña inválidos."));

        var token = tokenGenerator.GenerateToken(user);
        var expiresAtUtc = DateTimeOffset.UtcNow.AddMinutes(jwtOptions.Value.ExpiryMinutes);

        return Result.Success(new LoginResponse(token, expiresAtUtc, user.Id, user.Name, user.Email, user.Role.ToString()));
    }
}

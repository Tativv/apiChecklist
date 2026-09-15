using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Users.Create;

public sealed record CreateUserCommand(string Name, string Email, string Password, string Role) : ICommand<CreateUserResponse>;

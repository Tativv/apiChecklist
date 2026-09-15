using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Auth.Login;

public sealed record LoginCommand(string Email, string Password) : ICommand<LoginResponse>;

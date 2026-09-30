using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Auth.Login;

public sealed record LoginCommand(string Name, string Password) : ICommand<LoginResponse>;

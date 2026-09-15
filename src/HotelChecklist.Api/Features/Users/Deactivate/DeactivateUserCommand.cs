using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Users.Deactivate;

public sealed record DeactivateUserCommand(Guid Id) : ICommand<Unit>;

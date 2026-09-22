using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Users.Update;

public sealed record UpdateUserCommand(Guid Id, string Name, string Role, List<Guid> AreaIds) : ICommand<UpdateUserResponse>;

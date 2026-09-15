using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Users.GetById;

public sealed record GetUserByIdQuery(Guid Id) : IQuery<GetUserByIdResponse>;

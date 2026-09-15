using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.Areas.Create;

public sealed record CreateAreaCommand(string Name) : ICommand<CreateAreaResponse>;

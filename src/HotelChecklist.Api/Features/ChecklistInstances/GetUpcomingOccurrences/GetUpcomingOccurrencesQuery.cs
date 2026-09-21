using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.GetUpcomingOccurrences;

public sealed record GetUpcomingOccurrencesQuery(DateOnly From, DateOnly To) : IQuery<IReadOnlyList<UpcomingOccurrenceItem>>;

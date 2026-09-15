using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Delete;

public sealed record DeleteChecklistTemplateCommand(Guid Id) : ICommand<Unit>;

using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Features.ChecklistTemplates.Delete;

public sealed record DeleteChecklistTemplateCommand(Guid Id, UserRole ActingUserRole) : ICommand<Unit>;

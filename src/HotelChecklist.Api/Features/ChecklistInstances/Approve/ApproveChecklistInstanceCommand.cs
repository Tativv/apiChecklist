using HotelChecklist.Api.Common.Cqrs;

namespace HotelChecklist.Api.Features.ChecklistInstances.Approve;

public sealed record ApproveChecklistInstanceCommand(Guid Id, Guid ApprovedByUserId) : ICommand<ApproveChecklistInstanceResponse>;

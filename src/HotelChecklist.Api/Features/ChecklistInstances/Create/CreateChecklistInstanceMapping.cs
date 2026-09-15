using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Features.ChecklistInstances.Create;

public static class CreateChecklistInstanceMapping
{
    public static CreateChecklistInstanceCommand ToCommand(this CreateChecklistInstanceRequest request) =>
        new(request.TemplateId, request.AssetId, request.Date, request.AssignedUserId);

    public static CreateChecklistInstanceResponse ToResponse(this ChecklistInstance instance) =>
        new(instance.Id, instance.TemplateId, instance.AssetId, instance.Date, instance.Status.ToString(), instance.AssignedUserId);
}

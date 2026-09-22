using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;

namespace HotelChecklist.Api.Features.ChecklistInstances.Create;

public sealed class CreateChecklistInstanceHandler(AppDbContext db, ChecklistInstanceCreationService creationService)
    : ICommandHandler<CreateChecklistInstanceCommand, CreateChecklistInstanceResponse>
{
    public async Task<Result<CreateChecklistInstanceResponse>> Handle(CreateChecklistInstanceCommand command, CancellationToken cancellationToken)
    {
        var result = await creationService.CreateAsync(command.TemplateId, command.AssetId, command.Date, cancellationToken);

        if (result.IsFailure)
            return Result.Failure<CreateChecklistInstanceResponse>(result.Error);

        await db.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.ToResponse());
    }
}

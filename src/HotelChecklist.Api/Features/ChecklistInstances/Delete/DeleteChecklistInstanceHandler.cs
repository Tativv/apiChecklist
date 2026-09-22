using HotelChecklist.Api.Common.Adapters.FileStorage;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.Delete;

public sealed class DeleteChecklistInstanceHandler(AppDbContext db, IFileStorage fileStorage) : ICommandHandler<DeleteChecklistInstanceCommand, Unit>
{
    public async Task<Result<Unit>> Handle(DeleteChecklistInstanceCommand command, CancellationToken cancellationToken)
    {
        var instance = await db.ChecklistInstances.FindAsync([command.Id], cancellationToken);

        if (instance is null)
            return Result.Failure<Unit>(Error.NotFound("ChecklistInstances.NotFound", "Checklist no encontrado."));

        var evidenceFilePaths = await db.ChecklistTaskEvidences
            .Where(e => e.ChecklistTaskExecution.ChecklistInstanceId == command.Id)
            .Select(e => e.FilePath)
            .ToListAsync(cancellationToken);

        db.ChecklistInstances.Remove(instance);
        await db.SaveChangesAsync(cancellationToken);

        foreach (var filePath in evidenceFilePaths)
            await fileStorage.DeleteAsync(filePath, cancellationToken);

        return Result.Success(Unit.Value);
    }
}

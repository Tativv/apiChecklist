using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistInstances.GetById;

public sealed class GetChecklistInstanceByIdHandler(AppDbContext db) : IQueryHandler<GetChecklistInstanceByIdQuery, GetChecklistInstanceByIdResponse>
{
    public async Task<Result<GetChecklistInstanceByIdResponse>> Handle(GetChecklistInstanceByIdQuery query, CancellationToken cancellationToken)
    {
        var response = await db.ChecklistInstances
            .Where(i => i.Id == query.Id)
            .Select(i => new GetChecklistInstanceByIdResponse(
                i.Id,
                i.TemplateId,
                i.Template.Name,
                i.AssetId,
                i.Asset.Name,
                i.Date,
                i.Status.ToString(),
                i.StartedAt,
                i.CompletedAt,
                i.DurationSeconds,
                i.TaskExecutions
                    .OrderBy(e => e.Task.Order)
                    .Select(e => new TaskExecutionResponseItem(
                        e.Id, e.TaskId, e.Task.Name, e.Task.Order, e.Status.ToString(), e.ScheduledForUtc, e.Task.EstimatedDurationMinutes,
                        e.StartedAt, e.CompletedAt, e.DurationSeconds,
                        e.Comment, e.AssignedUserId, e.CreatedByUserId, e.ExecutedByUserId, e.ApprovedByUserId, e.ApprovedAt,
                        e.Comments.Count))
                    .ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        if (response is null)
            return Result.Failure<GetChecklistInstanceByIdResponse>(Error.NotFound("ChecklistInstances.NotFound", "Checklist no encontrado."));

        return Result.Success(response);
    }
}

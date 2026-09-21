using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Common.Persistence;
using HotelChecklist.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace HotelChecklist.Api.Features.ChecklistTemplates.GetById;

public sealed class GetChecklistTemplateByIdHandler(AppDbContext db) : IQueryHandler<GetChecklistTemplateByIdQuery, GetChecklistTemplateByIdResponse>
{
    public async Task<Result<GetChecklistTemplateByIdResponse>> Handle(GetChecklistTemplateByIdQuery query, CancellationToken cancellationToken)
    {
        var template = await db.ChecklistTemplates
            .Include(t => t.Tasks).ThenInclude(t => t.TaskSchedules).ThenInclude(ts => ts.Schedule)
            .Include(t => t.TemplateSchedules).ThenInclude(ts => ts.Schedule)
            .Include(t => t.TemplateAssets)
            .FirstOrDefaultAsync(t => t.Id == query.Id, cancellationToken);

        if (template is null)
            return Result.Failure<GetChecklistTemplateByIdResponse>(Error.NotFound("ChecklistTemplates.NotFound", "Template no encontrado."));

        return Result.Success(template.ToResponse());
    }
}

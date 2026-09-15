using FluentValidation;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Features.ChecklistInstances.Approve;
using HotelChecklist.Api.Features.ChecklistInstances.CompleteTask;
using HotelChecklist.Api.Features.ChecklistInstances.Create;
using HotelChecklist.Api.Features.ChecklistInstances.Finish;
using HotelChecklist.Api.Features.ChecklistInstances.GenerateDaily;
using HotelChecklist.Api.Features.ChecklistInstances.GetById;
using HotelChecklist.Api.Features.ChecklistInstances.GetEvidenceFile;
using HotelChecklist.Api.Features.ChecklistInstances.List;
using HotelChecklist.Api.Features.ChecklistInstances.Reopen;
using HotelChecklist.Api.Features.ChecklistInstances.Start;
using HotelChecklist.Api.Features.ChecklistInstances.UploadEvidence;

namespace HotelChecklist.Api.Features.ChecklistInstances;

public static class ChecklistInstancesEndpoints
{
    public static IServiceCollection AddChecklistInstancesFeature(this IServiceCollection services)
    {
        services.AddScoped<ChecklistInstanceCreationService>();

        services.AddScoped<ICommandHandler<CreateChecklistInstanceCommand, CreateChecklistInstanceResponse>, CreateChecklistInstanceHandler>();
        services.AddScoped<IValidator<CreateChecklistInstanceRequest>, CreateChecklistInstanceRequestValidator>();

        services.AddScoped<ICommandHandler<GenerateDailyChecklistsCommand, GenerateDailyChecklistsResponse>, GenerateDailyChecklistsHandler>();

        services.AddScoped<IQueryHandler<GetChecklistInstanceByIdQuery, GetChecklistInstanceByIdResponse>, GetChecklistInstanceByIdHandler>();

        services.AddScoped<IQueryHandler<ListChecklistInstancesQuery, IReadOnlyList<ListChecklistInstancesResponseItem>>, ListChecklistInstancesHandler>();

        services.AddScoped<ICommandHandler<StartChecklistInstanceCommand, StartChecklistInstanceResponse>, StartChecklistInstanceHandler>();

        services.AddScoped<ICommandHandler<CompleteChecklistTaskCommand, CompleteChecklistTaskResponse>, CompleteChecklistTaskHandler>();
        services.AddScoped<IValidator<CompleteChecklistTaskRequest>, CompleteChecklistTaskRequestValidator>();

        services.AddScoped<ICommandHandler<UploadEvidenceCommand, UploadEvidenceResponse>, UploadEvidenceHandler>();

        services.AddScoped<IQueryHandler<GetEvidenceFileQuery, GetEvidenceFileResponse>, GetEvidenceFileHandler>();

        services.AddScoped<ICommandHandler<FinishChecklistInstanceCommand, FinishChecklistInstanceResponse>, FinishChecklistInstanceHandler>();

        services.AddScoped<ICommandHandler<ApproveChecklistInstanceCommand, ApproveChecklistInstanceResponse>, ApproveChecklistInstanceHandler>();

        services.AddScoped<ICommandHandler<ReopenChecklistInstanceCommand, ReopenChecklistInstanceResponse>, ReopenChecklistInstanceHandler>();
        services.AddScoped<IValidator<ReopenChecklistInstanceRequest>, ReopenChecklistInstanceRequestValidator>();

        return services;
    }

    public static void MapChecklistInstancesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/checklist-instances").WithTags("ChecklistInstances");

        group.MapCreateChecklistInstance();
        group.MapGenerateDailyChecklists();
        group.MapGetChecklistInstanceById();
        group.MapListChecklistInstances();
        group.MapStartChecklistInstance();
        group.MapCompleteChecklistTask();
        group.MapUploadEvidence();
        group.MapGetEvidenceFile();
        group.MapFinishChecklistInstance();
        group.MapApproveChecklistInstance();
        group.MapReopenChecklistInstance();
    }
}

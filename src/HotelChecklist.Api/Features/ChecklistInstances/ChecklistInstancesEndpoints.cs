using FluentValidation;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Features.ChecklistInstances.AssignTask;
using HotelChecklist.Api.Features.ChecklistInstances.CompleteTask;
using HotelChecklist.Api.Features.ChecklistInstances.Create;
using HotelChecklist.Api.Features.ChecklistInstances.Delete;
using HotelChecklist.Api.Features.ChecklistInstances.Finish;
using HotelChecklist.Api.Features.ChecklistInstances.GenerateScheduled;
using HotelChecklist.Api.Features.ChecklistInstances.GetById;
using HotelChecklist.Api.Features.ChecklistInstances.GetEvidenceFile;
using HotelChecklist.Api.Features.ChecklistInstances.GetMyAssignedTasks;
using HotelChecklist.Api.Features.ChecklistInstances.GetUpcomingOccurrences;
using HotelChecklist.Api.Features.ChecklistInstances.List;
using HotelChecklist.Api.Features.ChecklistInstances.Reopen;
using HotelChecklist.Api.Features.ChecklistInstances.ReviewTask;
using HotelChecklist.Api.Features.ChecklistInstances.StartTask;
using HotelChecklist.Api.Features.ChecklistInstances.UploadEvidence;

namespace HotelChecklist.Api.Features.ChecklistInstances;

public static class ChecklistInstancesEndpoints
{
    public static IServiceCollection AddChecklistInstancesFeature(this IServiceCollection services)
    {
        services.AddScoped<ChecklistInstanceCreationService>();
        services.AddScoped<ScheduleEvaluationService>();

        services.AddScoped<ICommandHandler<CreateChecklistInstanceCommand, CreateChecklistInstanceResponse>, CreateChecklistInstanceHandler>();
        services.AddScoped<IValidator<CreateChecklistInstanceRequest>, CreateChecklistInstanceRequestValidator>();

        services.AddScoped<ICommandHandler<DeleteChecklistInstanceCommand, Unit>, DeleteChecklistInstanceHandler>();

        services.AddScoped<ICommandHandler<GenerateScheduledChecklistsCommand, GenerateScheduledChecklistsResponse>, GenerateScheduledChecklistsHandler>();

        services.AddScoped<IQueryHandler<GetUpcomingOccurrencesQuery, IReadOnlyList<UpcomingOccurrenceItem>>, GetUpcomingOccurrencesHandler>();

        services.AddScoped<IQueryHandler<GetChecklistInstanceByIdQuery, GetChecklistInstanceByIdResponse>, GetChecklistInstanceByIdHandler>();

        services.AddScoped<IQueryHandler<ListChecklistInstancesQuery, IReadOnlyList<ListChecklistInstancesResponseItem>>, ListChecklistInstancesHandler>();

        services.AddScoped<ICommandHandler<StartTaskCommand, StartTaskResponse>, StartTaskHandler>();

        services.AddScoped<ICommandHandler<CompleteChecklistTaskCommand, CompleteChecklistTaskResponse>, CompleteChecklistTaskHandler>();
        services.AddScoped<IValidator<CompleteChecklistTaskRequest>, CompleteChecklistTaskRequestValidator>();

        services.AddScoped<ICommandHandler<ReviewTaskCommand, ReviewTaskResponse>, ReviewTaskHandler>();

        services.AddScoped<ICommandHandler<AssignTaskCommand, AssignTaskResponse>, AssignTaskHandler>();

        services.AddScoped<IQueryHandler<GetMyAssignedTasksQuery, IReadOnlyList<MyAssignedTaskItem>>, GetMyAssignedTasksHandler>();

        services.AddScoped<ICommandHandler<UploadEvidenceCommand, UploadEvidenceResponse>, UploadEvidenceHandler>();

        services.AddScoped<IQueryHandler<GetEvidenceFileQuery, GetEvidenceFileResponse>, GetEvidenceFileHandler>();

        services.AddScoped<ICommandHandler<FinishChecklistInstanceCommand, FinishChecklistInstanceResponse>, FinishChecklistInstanceHandler>();

        services.AddScoped<ICommandHandler<ReopenChecklistInstanceCommand, ReopenChecklistInstanceResponse>, ReopenChecklistInstanceHandler>();
        services.AddScoped<IValidator<ReopenChecklistInstanceRequest>, ReopenChecklistInstanceRequestValidator>();

        return services;
    }

    public static void MapChecklistInstancesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/checklist-instances").WithTags("ChecklistInstances");

        group.MapCreateChecklistInstance();
        group.MapDeleteChecklistInstance();
        group.MapGenerateScheduledChecklists();
        group.MapGetUpcomingOccurrences();
        group.MapGetMyAssignedTasks();
        group.MapGetChecklistInstanceById();
        group.MapListChecklistInstances();
        group.MapStartTask();
        group.MapCompleteChecklistTask();
        group.MapReviewTask();
        group.MapAssignTask();
        group.MapUploadEvidence();
        group.MapGetEvidenceFile();
        group.MapFinishChecklistInstance();
        group.MapReopenChecklistInstance();
    }
}

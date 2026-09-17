using FluentValidation;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Features.ChecklistTemplates.ConfigureAssets;
using HotelChecklist.Api.Features.ChecklistTemplates.Create;
using HotelChecklist.Api.Features.ChecklistTemplates.Delete;
using HotelChecklist.Api.Features.ChecklistTemplates.GetById;
using HotelChecklist.Api.Features.ChecklistTemplates.List;
using HotelChecklist.Api.Features.ChecklistTemplates.Update;

namespace HotelChecklist.Api.Features.ChecklistTemplates;

public static class ChecklistTemplatesEndpoints
{
    public static IServiceCollection AddChecklistTemplatesFeature(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateChecklistTemplateCommand, CreateChecklistTemplateResponse>, CreateChecklistTemplateHandler>();
        services.AddScoped<IValidator<CreateChecklistTemplateRequest>, CreateChecklistTemplateRequestValidator>();

        services.AddScoped<IQueryHandler<GetChecklistTemplateByIdQuery, GetChecklistTemplateByIdResponse>, GetChecklistTemplateByIdHandler>();

        services.AddScoped<IQueryHandler<ListChecklistTemplatesQuery, IReadOnlyList<ListChecklistTemplatesResponseItem>>, ListChecklistTemplatesHandler>();

        services.AddScoped<ICommandHandler<UpdateChecklistTemplateCommand, UpdateChecklistTemplateResponse>, UpdateChecklistTemplateHandler>();
        services.AddScoped<IValidator<UpdateChecklistTemplateRequest>, UpdateChecklistTemplateRequestValidator>();

        services.AddScoped<ICommandHandler<DeleteChecklistTemplateCommand, Unit>, DeleteChecklistTemplateHandler>();

        services.AddScoped<ICommandHandler<ConfigureTemplateAssetsCommand, ConfigureTemplateAssetsResponse>, ConfigureTemplateAssetsHandler>();
        services.AddScoped<IValidator<ConfigureTemplateAssetsRequest>, ConfigureTemplateAssetsRequestValidator>();

        return services;
    }

    public static void MapChecklistTemplatesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/checklist-templates").WithTags("ChecklistTemplates");

        group.MapCreateChecklistTemplate();
        group.MapGetChecklistTemplateById();
        group.MapListChecklistTemplates();
        group.MapUpdateChecklistTemplate();
        group.MapDeleteChecklistTemplate();
        group.MapConfigureTemplateAssets();
    }
}

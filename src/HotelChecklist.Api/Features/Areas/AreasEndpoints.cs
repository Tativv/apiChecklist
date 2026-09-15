using FluentValidation;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Features.Areas.Create;
using HotelChecklist.Api.Features.Areas.Delete;
using HotelChecklist.Api.Features.Areas.GetById;
using HotelChecklist.Api.Features.Areas.List;
using HotelChecklist.Api.Features.Areas.Update;

namespace HotelChecklist.Api.Features.Areas;

public static class AreasEndpoints
{
    public static IServiceCollection AddAreasFeature(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateAreaCommand, CreateAreaResponse>, CreateAreaHandler>();
        services.AddScoped<IValidator<CreateAreaRequest>, CreateAreaRequestValidator>();

        services.AddScoped<IQueryHandler<GetAreaByIdQuery, GetAreaByIdResponse>, GetAreaByIdHandler>();

        services.AddScoped<IQueryHandler<ListAreasQuery, IReadOnlyList<ListAreasResponseItem>>, ListAreasHandler>();

        services.AddScoped<ICommandHandler<UpdateAreaCommand, UpdateAreaResponse>, UpdateAreaHandler>();
        services.AddScoped<IValidator<UpdateAreaRequest>, UpdateAreaRequestValidator>();

        services.AddScoped<ICommandHandler<DeleteAreaCommand, Unit>, DeleteAreaHandler>();

        return services;
    }

    public static void MapAreasEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/areas").WithTags("Areas");

        group.MapCreateArea();
        group.MapGetAreaById();
        group.MapListAreas();
        group.MapUpdateArea();
        group.MapDeleteArea();
    }
}

using FluentValidation;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Features.Assets.Create;
using HotelChecklist.Api.Features.Assets.Delete;
using HotelChecklist.Api.Features.Assets.GetById;
using HotelChecklist.Api.Features.Assets.List;
using HotelChecklist.Api.Features.Assets.Update;

namespace HotelChecklist.Api.Features.Assets;

public static class AssetsEndpoints
{
    public static IServiceCollection AddAssetsFeature(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateAssetCommand, CreateAssetResponse>, CreateAssetHandler>();
        services.AddScoped<IValidator<CreateAssetRequest>, CreateAssetRequestValidator>();

        services.AddScoped<IQueryHandler<GetAssetByIdQuery, GetAssetByIdResponse>, GetAssetByIdHandler>();

        services.AddScoped<IQueryHandler<ListAssetsQuery, IReadOnlyList<ListAssetsResponseItem>>, ListAssetsHandler>();

        services.AddScoped<ICommandHandler<UpdateAssetCommand, UpdateAssetResponse>, UpdateAssetHandler>();
        services.AddScoped<IValidator<UpdateAssetRequest>, UpdateAssetRequestValidator>();

        services.AddScoped<ICommandHandler<DeleteAssetCommand, Unit>, DeleteAssetHandler>();

        return services;
    }

    public static void MapAssetsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assets").WithTags("Assets");

        group.MapCreateAsset();
        group.MapGetAssetById();
        group.MapListAssets();
        group.MapUpdateAsset();
        group.MapDeleteAsset();
    }
}

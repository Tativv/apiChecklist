using FluentValidation;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Features.Auth.Login;

namespace HotelChecklist.Api.Features.Auth;

public static class AuthEndpoints
{
    public static IServiceCollection AddAuthFeature(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<LoginCommand, LoginResponse>, LoginHandler>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();

        return services;
    }

    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapLogin();
    }
}

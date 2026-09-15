using FluentValidation;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Features.Users.Create;
using HotelChecklist.Api.Features.Users.Deactivate;
using HotelChecklist.Api.Features.Users.GetById;
using HotelChecklist.Api.Features.Users.List;
using HotelChecklist.Api.Features.Users.Update;

namespace HotelChecklist.Api.Features.Users;

public static class UsersEndpoints
{
    public static IServiceCollection AddUsersFeature(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateUserCommand, CreateUserResponse>, CreateUserHandler>();
        services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();

        services.AddScoped<IQueryHandler<GetUserByIdQuery, GetUserByIdResponse>, GetUserByIdHandler>();

        services.AddScoped<IQueryHandler<ListUsersQuery, IReadOnlyList<ListUsersResponseItem>>, ListUsersHandler>();

        services.AddScoped<ICommandHandler<UpdateUserCommand, UpdateUserResponse>, UpdateUserHandler>();
        services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();

        services.AddScoped<ICommandHandler<DeactivateUserCommand, Unit>, DeactivateUserHandler>();

        return services;
    }

    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapCreateUser();
        group.MapGetUserById();
        group.MapListUsers();
        group.MapUpdateUser();
        group.MapDeactivateUser();
    }
}

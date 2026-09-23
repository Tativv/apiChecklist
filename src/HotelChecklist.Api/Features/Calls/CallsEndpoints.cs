using FluentValidation;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Features.Calls.Assign;
using HotelChecklist.Api.Features.Calls.Create;
using HotelChecklist.Api.Features.Calls.Finish;
using HotelChecklist.Api.Features.Calls.GetById;
using HotelChecklist.Api.Features.Calls.List;
using HotelChecklist.Api.Features.Calls.Start;

namespace HotelChecklist.Api.Features.Calls;

public static class CallsEndpoints
{
    public static IServiceCollection AddCallsFeature(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateCallCommand, CreateCallResponse>, CreateCallHandler>();
        services.AddScoped<IValidator<CreateCallRequest>, CreateCallRequestValidator>();

        services.AddScoped<IQueryHandler<ListCallsQuery, IReadOnlyList<ListCallsResponseItem>>, ListCallsHandler>();

        services.AddScoped<IQueryHandler<GetCallByIdQuery, GetCallByIdResponse>, GetCallByIdHandler>();

        services.AddScoped<ICommandHandler<AssignCallCommand, AssignCallResponse>, AssignCallHandler>();

        services.AddScoped<ICommandHandler<StartCallCommand, StartCallResponse>, StartCallHandler>();

        services.AddScoped<ICommandHandler<FinishCallCommand, FinishCallResponse>, FinishCallHandler>();

        return services;
    }

    public static void MapCallsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/calls").WithTags("Calls");

        group.MapCreateCall();
        group.MapListCalls();
        group.MapGetCallById();
        group.MapAssignCall();
        group.MapStartCall();
        group.MapFinishCall();
    }
}

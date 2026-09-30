using FluentValidation;
using HotelChecklist.Api.Common.Cqrs;
using HotelChecklist.Api.Features.ServiceOrders.Assign;
using HotelChecklist.Api.Features.ServiceOrders.Create;
using HotelChecklist.Api.Features.ServiceOrders.Delete;
using HotelChecklist.Api.Features.ServiceOrders.Finish;
using HotelChecklist.Api.Features.ServiceOrders.GetById;
using HotelChecklist.Api.Features.ServiceOrders.List;
using HotelChecklist.Api.Features.ServiceOrders.ServiceOrderComments;
using HotelChecklist.Api.Features.ServiceOrders.Start;

namespace HotelChecklist.Api.Features.ServiceOrders;

public static class ServiceOrdersEndpoints
{
    public static IServiceCollection AddServiceOrdersFeature(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateServiceOrderCommand, CreateServiceOrderResponse>, CreateServiceOrderHandler>();
        services.AddScoped<IValidator<CreateServiceOrderRequest>, CreateServiceOrderRequestValidator>();

        services.AddScoped<IQueryHandler<ListServiceOrdersQuery, IReadOnlyList<ListServiceOrdersResponseItem>>, ListServiceOrdersHandler>();

        services.AddScoped<IQueryHandler<GetServiceOrderByIdQuery, GetServiceOrderByIdResponse>, GetServiceOrderByIdHandler>();

        services.AddScoped<ICommandHandler<AssignServiceOrderCommand, AssignServiceOrderResponse>, AssignServiceOrderHandler>();

        services.AddScoped<ICommandHandler<StartServiceOrderCommand, StartServiceOrderResponse>, StartServiceOrderHandler>();

        services.AddScoped<ICommandHandler<FinishServiceOrderCommand, FinishServiceOrderResponse>, FinishServiceOrderHandler>();

        services.AddScoped<IQueryHandler<ListServiceOrderCommentsQuery, IReadOnlyList<ServiceOrderCommentResponseItem>>, ListServiceOrderCommentsHandler>();

        services.AddScoped<ICommandHandler<AddServiceOrderCommentCommand, ServiceOrderCommentResponseItem>, AddServiceOrderCommentHandler>();

        services.AddScoped<IQueryHandler<GetServiceOrderCommentFileQuery, GetServiceOrderCommentFileResponse>, GetServiceOrderCommentFileHandler>();

        services.AddScoped<ICommandHandler<DeleteServiceOrderCommand, Unit>, DeleteServiceOrderHandler>();

        return services;
    }

    public static void MapServiceOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/service-orders").WithTags("ServiceOrders");

        group.MapCreateServiceOrder();
        group.MapListServiceOrders();
        group.MapGetServiceOrderById();
        group.MapAssignServiceOrder();
        group.MapStartServiceOrder();
        group.MapFinishServiceOrder();
        group.MapListServiceOrderComments();
        group.MapAddServiceOrderComment();
        group.MapGetServiceOrderCommentFile();
        group.MapDeleteServiceOrder();
    }
}

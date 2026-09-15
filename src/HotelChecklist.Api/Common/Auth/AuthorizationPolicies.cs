using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Common.Auth;

public static class Policies
{
    public const string AdminOnly = nameof(AdminOnly);
    public const string ManagerOrAbove = nameof(ManagerOrAbove);
    public const string SupervisorOrAbove = nameof(SupervisorOrAbove);
    public const string AnyRole = nameof(AnyRole);
}

public static class AuthorizationPolicySetup
{
    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.AdminOnly, policy => policy.RequireRole(UserRole.Admin.ToString()))
            .AddPolicy(Policies.ManagerOrAbove, policy => policy.RequireRole(
                UserRole.Admin.ToString(), UserRole.Manager.ToString()))
            .AddPolicy(Policies.SupervisorOrAbove, policy => policy.RequireRole(
                UserRole.Admin.ToString(), UserRole.Manager.ToString(), UserRole.Supervisor.ToString()))
            .AddPolicy(Policies.AnyRole, policy => policy.RequireRole(
                UserRole.Admin.ToString(), UserRole.Manager.ToString(), UserRole.Supervisor.ToString(), UserRole.Operator.ToString()));

        return services;
    }
}

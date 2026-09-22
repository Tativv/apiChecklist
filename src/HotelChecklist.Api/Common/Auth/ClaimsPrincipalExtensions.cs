using System.Security.Claims;
using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Api.Common.Auth;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var subClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub");
        return Guid.Parse(subClaim!);
    }

    public static bool IsSupervisorOrAbove(this ClaimsPrincipal principal) =>
        principal.IsInRole(UserRole.Directoria.ToString())
        || principal.IsInRole(UserRole.Gerencia.ToString())
        || principal.IsInRole(UserRole.Supervisor.ToString());

    public static bool IsExactlySupervisor(this ClaimsPrincipal principal) =>
        principal.IsInRole(UserRole.Supervisor.ToString())
        && !principal.IsInRole(UserRole.Directoria.ToString())
        && !principal.IsInRole(UserRole.Gerencia.ToString());
}

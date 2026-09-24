using HotelChecklist.Domain.Enums;

namespace HotelChecklist.Domain.Common;

public static class RoleHierarchy
{
    public static int Rank(UserRole role) => role switch
    {
        UserRole.Colaborador => 0,
        UserRole.Supervisor => 1,
        UserRole.Gerencia => 2,
        UserRole.Directoria => 3,
        _ => 0
    };

    public static bool Outranks(UserRole actingRole, UserRole requiredRole) => Rank(actingRole) >= Rank(requiredRole);
}

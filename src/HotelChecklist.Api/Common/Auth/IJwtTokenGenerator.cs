using HotelChecklist.Domain.Entities;

namespace HotelChecklist.Api.Common.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}

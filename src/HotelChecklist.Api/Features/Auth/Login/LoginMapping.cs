namespace HotelChecklist.Api.Features.Auth.Login;

public static class LoginMapping
{
    public static LoginCommand ToCommand(this LoginRequest request) => new(request.Email, request.Password);
}

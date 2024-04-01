using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public static class ApiMappings
{
    public static void AddLoginAndUserEndpoints(this IEndpointRouteBuilder builder)
    {
        var login = builder.MapGroup("api/login");

        login.MapPost("/", async ([FromBody] LoginModel loginModel, IUserService userService) =>
            await userService.AuthenticateUser(loginModel))
            .AddProduces<string>(401, 500);

        var user = builder.MapGroup("api/user");

        user.MapPost("/", async ([FromBody] LoginModel loginModel, IUserService userService) =>
            await userService.CreateUser(loginModel))
            .AddProduces<string>(400, 500);
    }
}
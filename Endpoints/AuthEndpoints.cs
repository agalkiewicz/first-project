using FluentValidation;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var authApi = routes.MapGroup("/api/auth").WithTags("Auth");

        authApi.MapPost("/register", async (UserRegistrationRequest request, IValidator<UserRegistrationRequest> validator, IUserService userService) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            
            var result = await userService.RegisterAsync(request);
            return Results.Ok(result);
        });

        authApi.MapPost("/token", async (TokenRequest request, IUserService userService) =>
        {
            var result = await userService.GetTokenAsync(request);
            if (!result.IsAuthenticated)
            {
                return Results.BadRequest(result.Message);
            }
            return Results.Ok(result);
        });
    }
}

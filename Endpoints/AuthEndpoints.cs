using FluentValidation;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var authApi = routes.MapGroup("/api/auth").WithTags("Auth");

        authApi.MapPost("/register", async (UserRegistrationRequest request, IValidator<UserRegistrationRequest> validator) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            // perform actual service call to register the user to the system
            // _service.RegisterUser(request);
            return Results.Accepted();
        });
    }
}

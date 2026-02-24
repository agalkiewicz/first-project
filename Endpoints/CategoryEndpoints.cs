using Movies.Api.Common;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var categoryApi = routes.MapGroup("/api/categories").WithTags("Categories");

        categoryApi.MapPost("/", async (CreateCategoryDto command, ICategoryService service) =>
        {
            var category = await service.CreateCategoryAsync(command);
            return TypedResults.Created($"/api/categories/{category.Id}", category);
        })
        .AddEndpointFilter(new ValidationFilter<CreateCategoryDto>())
        .RequireAuthorization();

        categoryApi.MapGet("/", async (ICategoryService service) =>
        {
            var categories = await service.GetAllCategoriesAsync();
            return TypedResults.Ok(categories);
        });

        categoryApi.MapGet("/{id}", async (ICategoryService service, int id) =>
        {
            var category = await service.GetCategoryByIdAsync(id);

            return category is null
                ? (IResult)TypedResults.NotFound(new { Message = $"Category with ID {id} not found." })
                : TypedResults.Ok(category);
        });

        categoryApi.MapPut("/{id}", async (ICategoryService service, int id, UpdateCategoryDto command) =>
        {
            await service.UpdateCategoryAsync(id, command);
            return TypedResults.NoContent();
        })
        .AddEndpointFilter(new ValidationFilter<UpdateCategoryDto>())
        .RequireAuthorization();

        categoryApi.MapDelete("/{id}", async (ICategoryService service, int id) =>
        {
            await service.DeleteCategoryAsync(id);
            return TypedResults.NoContent();
        });
    }
}

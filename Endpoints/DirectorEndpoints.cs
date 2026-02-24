using Movies.Api.Common;

public static class DirectorEndpoints
{
    public static void MapDirectorEndpoints(this IEndpointRouteBuilder routes)
    {
        var directorApi = routes.MapGroup("/api/directors").WithTags("Directors");

        directorApi.MapPost("/", async (CreateDirectorDto command, IDirectorService service) =>
        {
            var director = await service.CreateDirectorAsync(command);
            return TypedResults.Created($"/api/directors/{director.Id}", director);
        })
        .RequireAuthorization();

        directorApi.MapGet("/", async (IDirectorService service) =>
        {
            var directors = await service.GetAllDirectorsAsync();
            return TypedResults.Ok(directors);
        });

        directorApi.MapGet("/{id}", async (IDirectorService service, int id) =>
        {
            var director = await service.GetDirectorByIdAsync(id);

            return director is null
                ? (IResult)TypedResults.NotFound(new { Message = $"Director with ID {id} not found." })
                : TypedResults.Ok(director);
        });

        directorApi.MapGet("/{id}/movies", async (IDirectorService service, int id, [AsParameters] MovieQueryFilter filter) =>
        {
            var movies = await service.GetMoviesByDirectorIdAsync(id, filter);
            return TypedResults.Ok(movies);
        });

        directorApi.MapPut("/{id}", async (IDirectorService service, int id, UpdateDirectorDto command) =>
        {
            await service.UpdateDirectorAsync(id, command);
            return TypedResults.NoContent();
        })
        .RequireAuthorization();

        directorApi.MapDelete("/{id}", async (IDirectorService service, int id) =>
        {
            await service.DeleteDirectorAsync(id);
            return TypedResults.NoContent();
        });
    }
}

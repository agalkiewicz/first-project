using Movies.Api.Common;

public static class MovieEndpoints
{
    public static void MapMovieEndpoints(this IEndpointRouteBuilder routes)
    {
        var movieApi = routes.MapGroup("/api/movies").WithTags("Movies");

        movieApi.MapPost("/", async (CreateMovieDto command, IMovieService service) =>
        {
            var movie = await service.CreateMovieAsync(command);
            return TypedResults.Created($"/api/movies/{movie.Id}", movie);
        })
        .AddEndpointFilter(new ValidationFilter<CreateMovieDto>())
        .RequireAuthorization();

        movieApi.MapGet("/", async (
            [AsParameters] MovieQueryFilter filter,
            IMovieService service) =>
        {
            var result = await service.GetAllMoviesAsync(filter);
            return TypedResults.Ok(result);
        });

        movieApi.MapGet("/{id}", async (IMovieService service, int id) =>
        {
            var movie = await service.GetMovieByIdAsync(id);

            return movie is null
                ? (IResult)TypedResults.NotFound(new { Message = $"Movie with ID {id} not found." })
                : TypedResults.Ok(movie);
        });

        movieApi.MapPut("/{id}", async (IMovieService service, int id, UpdateMovieDto command) =>
        {
            await service.UpdateMovieAsync(id, command);
            return TypedResults.NoContent();
        })
        .RequireAuthorization();

        movieApi.MapDelete("/{id}", async (IMovieService service, int id) =>
        {
            await service.DeleteMovieAsync(id);
            return TypedResults.NoContent();
        });
    }
}

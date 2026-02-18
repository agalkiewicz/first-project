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
        });

        movieApi.MapGet("/", async (
            [AsParameters] MovieQueryFilter filter,
            IMovieService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.GetAllMoviesAsync(filter, cancellationToken);
            return TypedResults.Ok(result);
        });

        movieApi.MapGet("/{id}", async (IMovieService service, Guid id) =>
        {
            var movie = await service.GetMovieByIdAsync(id);

            return movie is null
                ? (IResult)TypedResults.NotFound(new { Message = $"Movie with ID {id} not found." })
                : TypedResults.Ok(movie);
        });

        movieApi.MapPut("/{id}", async (IMovieService service, Guid id, UpdateMovieDto command) =>
        {
            await service.UpdateMovieAsync(id, command);
            return TypedResults.NoContent();
        });

        movieApi.MapDelete("/{id}", async (IMovieService service, Guid id) =>
        {
            await service.DeleteMovieAsync(id);
            return TypedResults.NoContent();
        });
    }
}

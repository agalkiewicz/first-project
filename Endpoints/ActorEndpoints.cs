using Movies.Api.Common;

public static class ActorEndpoints
{
    public static void MapActorEndpoints(this IEndpointRouteBuilder routes)
    {
        var actorApi = routes.MapGroup("/api/actors").WithTags("Actors");

        actorApi.MapPost("/", async (CreateActorDto command, IActorService service) =>
        {
            var actor = await service.CreateActorAsync(command);
            return TypedResults.Created($"/api/actors/{actor.Id}", actor);
        })
        .RequireAuthorization();

        actorApi.MapGet("/", async (IActorService service) =>
        {
            var actors = await service.GetAllActorsAsync();
            return TypedResults.Ok(actors);
        });

        actorApi.MapGet("/{id}", async (IActorService service, int id) =>
        {
            var actor = await service.GetActorByIdAsync(id);

            return actor is null
                ? (IResult)TypedResults.NotFound(new { Message = $"Actor with ID {id} not found." })
                : TypedResults.Ok(actor);
        });

        actorApi.MapGet("/{id}/movies", async (IActorService service, int id, [AsParameters] MovieQueryFilter filter) =>
        {
            var movies = await service.GetMoviesByActorIdAsync(id, filter);
            return TypedResults.Ok(movies);
        });

        actorApi.MapPut("/{id}", async (IActorService service, int id, UpdateActorDto command) =>
        {
            await service.UpdateActorAsync(id, command);
            return TypedResults.NoContent();
        })
        .RequireAuthorization();

        actorApi.MapDelete("/{id}", async (IActorService service, int id) =>
        {
            await service.DeleteActorAsync(id);
            return TypedResults.NoContent();
        });
    }
}

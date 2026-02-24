using Movies.Api.Common;

public interface IActorService
{
    Task<ActorDto> CreateActorAsync(CreateActorDto command);
    Task<IReadOnlyCollection<ActorDto>> GetAllActorsAsync();
    Task<ActorDto?> GetActorByIdAsync(int id);
    Task<PagedResponse<MovieSummaryDto>> GetMoviesByActorIdAsync(int id, MovieQueryFilter filter);
    Task UpdateActorAsync(int id, UpdateActorDto command);
    Task DeleteActorAsync(int id);
}

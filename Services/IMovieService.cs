using Movies.Api.Common;

public interface IMovieService
{
    Task<MovieDto> CreateMovieAsync(CreateMovieDto command);
    Task<MovieDto?> GetMovieByIdAsync(Guid id);
    Task<PagedResponse<MovieDto>> GetAllMoviesAsync(MovieQueryFilter filter, CancellationToken cancellationToken = default);
    Task UpdateMovieAsync(Guid id, UpdateMovieDto command);
    Task DeleteMovieAsync(Guid id);
}

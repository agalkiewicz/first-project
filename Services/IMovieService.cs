using Movies.Api.Common;

public interface IMovieService
{
    Task<MovieDto> CreateMovieAsync(CreateMovieDto command);
    Task<MovieDto?> GetMovieByIdAsync(int id);
    Task<PagedResponse<MovieDto>> GetAllMoviesAsync(MovieQueryFilter filter, CancellationToken cancellationToken = default);
    Task UpdateMovieAsync(int id, UpdateMovieDto command);
    Task DeleteMovieAsync(int id);
}

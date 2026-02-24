using Movies.Api.Common;

public interface IDirectorService
{
    Task<DirectorDto> CreateDirectorAsync(CreateDirectorDto command);
    Task<IReadOnlyCollection<DirectorDto>> GetAllDirectorsAsync();
    Task<DirectorDto?> GetDirectorByIdAsync(int id);
    Task<PagedResponse<MovieSummaryDto>> GetMoviesByDirectorIdAsync(int id, MovieQueryFilter filter);
    Task UpdateDirectorAsync(int id, UpdateDirectorDto command);
    Task DeleteDirectorAsync(int id);
}

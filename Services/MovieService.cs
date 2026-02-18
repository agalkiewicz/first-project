using Microsoft.EntityFrameworkCore;
using Movies.Api.Common;

public class MovieService : IMovieService
{
    private readonly MovieDbContext _dbContext;
    private readonly ILogger<MovieService> _logger;

    public MovieService(MovieDbContext dbContext, ILogger<MovieService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<MovieDto> CreateMovieAsync(CreateMovieDto command)
    {
        var movie = Movie.Create(command.Title, command.Genre, command.ReleaseDate, command.Rating);

        await _dbContext.Movies.AddAsync(movie);
        await _dbContext.SaveChangesAsync();

        return new MovieDto(
           movie.Id,
           movie.Title,
           movie.Genre,
           movie.ReleaseDate,
           movie.Rating
        );
    }

    public async Task<PagedResponse<MovieDto>> GetAllMoviesAsync(
    MovieQueryFilter filter, CancellationToken cancellationToken = default)
    {
        var pageNumber = Math.Max(1, filter.PageNumber ?? 1);
        var pageSize = Math.Clamp(filter.PageSize ?? 10, 1, 50);

        var query = _dbContext.Movies.AsNoTracking().AsQueryable();

        // 1. Apply search filter (reduces the dataset)
        query = query.ApplySearch(filter.Search);

        // 2. Count total records AFTER filtering, BEFORE pagination
        var totalRecords = await query.CountAsync(cancellationToken);

        // 3. Apply sorting (default to Title if not specified)
        query = query.ApplySort(
            string.IsNullOrWhiteSpace(filter.SortBy) ? "Title" : filter.SortBy);

        // 4. Apply pagination and project to DTOs
        var movies = await query
            .ApplyPagination(pageNumber, pageSize)
            .Select(m => new MovieDto(m.Id, m.Title, m.Genre, m.ReleaseDate, m.Rating))
            .ToListAsync(cancellationToken);

        return new PagedResponse<MovieDto>
        {
            Data = movies,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }

    public async Task<MovieDto?> GetMovieByIdAsync(Guid id)
    {
        var movie = await _dbContext.Movies
                               .AsNoTracking()
                               .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
            return null;

        return new MovieDto(
            movie.Id,
            movie.Title,
            movie.Genre,
            movie.ReleaseDate,
            movie.Rating
        );
    }

    public async Task UpdateMovieAsync(Guid id, UpdateMovieDto command)
    {
        var movieToUpdate = await _dbContext.Movies.FindAsync(id);
        if (movieToUpdate is null)
            throw new ArgumentNullException($"Invalid Movie Id.");
        movieToUpdate.Update(command.Title, command.Genre, command.ReleaseDate, command.Rating);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteMovieAsync(Guid id)
    {
        var movieToDelete = await _dbContext.Movies.FindAsync(id);
        if (movieToDelete != null)
        {
            _dbContext.Movies.Remove(movieToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}

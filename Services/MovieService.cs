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
        var categories = await GetCategoriesByIdsAsync(command.CategoryIds ?? Array.Empty<int>());
        var movie = Movie.Create(command.Title, command.ReleaseDate, command.Rating, categories);

        await _dbContext.Movies.AddAsync(movie);
        await _dbContext.SaveChangesAsync();

        return new MovieDto(
           movie.Id,
           movie.Title,
           movie.ReleaseDate,
            movie.Rating,
            movie.Categories.Select(c => new CategoryDto(c.Id, c.Name)).ToList()
        );
    }

    public async Task<PagedResponse<MovieDto>> GetAllMoviesAsync(MovieQueryFilter filter)
    {
        var pageNumber = Math.Max(1, filter.PageNumber ?? 1);
        var pageSize = Math.Clamp(filter.PageSize ?? 10, 1, 50);

        var query = _dbContext.Movies
            .AsNoTracking()
            .AsQueryable();

        // 1. Apply search filter (reduces the dataset)
        query = query.ApplySearch(filter.Search);

        // 2. Count total records AFTER filtering, BEFORE pagination
        var totalRecords = await query.CountAsync();

        // 3. Apply sorting (default to Title if not specified)
        query = query.ApplySort(
            string.IsNullOrWhiteSpace(filter.SortBy) ? "Title" : filter.SortBy);

        // 4. Apply pagination and project to DTOs
        var movies = await query
            .ApplyPagination(pageNumber, pageSize)
            .Select(m => new MovieDto(
                m.Id,
                m.Title,
                m.ReleaseDate,
                m.Rating,
                m.Categories
                    .Select(c => new CategoryDto(c.Id, c.Name))
                    .ToList()))
            .ToListAsync();

        return new PagedResponse<MovieDto>
        {
            Data = movies,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }

    public async Task<MovieDto?> GetMovieByIdAsync(int id)
    {
        var movie = await _dbContext.Movies
                               .AsNoTracking()
                               .Include(m => m.Categories)
                               .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
            return null;

        return new MovieDto(
            movie.Id,
            movie.Title,
            movie.ReleaseDate,
            movie.Rating,
            movie.Categories.Select(c => new CategoryDto(c.Id, c.Name)).ToList()
        );
    }

    public async Task UpdateMovieAsync(int id, UpdateMovieDto command)
    {
        var movieToUpdate = await _dbContext.Movies.FindAsync(id);
        if (movieToUpdate is null)
            throw new ArgumentNullException($"Invalid Movie Id.");

        var categories = await GetCategoriesByIdsAsync(command.CategoryIds ?? Array.Empty<int>());
        movieToUpdate.Update(command.Title, command.ReleaseDate, command.Rating, categories);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteMovieAsync(int id)
    {
        var movieToDelete = await _dbContext.Movies.FindAsync(id);
        if (movieToDelete != null)
        {
            _dbContext.Movies.Remove(movieToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task<List<Category>> GetCategoriesByIdsAsync(IReadOnlyCollection<int> categoryIds)
    {
        var distinctIds = categoryIds.Distinct().ToArray();
        var categories = await _dbContext.Categories
            .Where(c => distinctIds.Contains(c.Id))
            .ToListAsync();

        if (categories.Count != distinctIds.Length)
            throw new ArgumentException("One or more category IDs are invalid.");

        return categories;
    }
}

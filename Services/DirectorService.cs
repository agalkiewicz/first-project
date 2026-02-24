using Microsoft.EntityFrameworkCore;
using Movies.Api.Common;

public class DirectorService : IDirectorService
{
    private readonly MovieDbContext _dbContext;

    public DirectorService(MovieDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DirectorDto> CreateDirectorAsync(CreateDirectorDto command)
    {
        var director = Director.Create(command.FirstName, command.LastName, command.DateOfBirth);

        await _dbContext.Directors.AddAsync(director);
        await _dbContext.SaveChangesAsync();

        return new DirectorDto(director.Id, director.FirstName, director.LastName, director.DateOfBirth);
    }

    public async Task<IReadOnlyCollection<DirectorDto>> GetAllDirectorsAsync()
    {
        return await _dbContext.Directors
            .AsNoTracking()
            .OrderBy(d => d.LastName)
            .ThenBy(d => d.FirstName)
            .Select(d => new DirectorDto(d.Id, d.FirstName, d.LastName, d.DateOfBirth))
            .ToListAsync();
    }

    public async Task<DirectorDto?> GetDirectorByIdAsync(int id)
    {
        return await _dbContext.Directors
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(d => new DirectorDto(d.Id, d.FirstName, d.LastName, d.DateOfBirth))
            .FirstOrDefaultAsync();
    }

    public async Task<PagedResponse<MovieSummaryDto>> GetMoviesByDirectorIdAsync(int id, MovieQueryFilter filter)
    {
        var pageNumber = Math.Max(1, filter.PageNumber ?? 1);
        var pageSize = Math.Clamp(filter.PageSize ?? 10, 1, 50);

        var query = _dbContext.Movies
            .AsNoTracking()
            .Where(m => m.DirectorId == id)
            .AsQueryable();

        query = query.ApplySearch(filter.Search);

        var totalRecords = await query.CountAsync();

        query = query.ApplySort(
            string.IsNullOrWhiteSpace(filter.SortBy) ? "Title" : filter.SortBy);

        var movies = await query
            .ApplyPagination(pageNumber, pageSize)
            .Select(m => new MovieSummaryDto(m.Id, m.Title, m.ReleaseDate, m.Rating))
            .ToListAsync();

        return new PagedResponse<MovieSummaryDto>
        {
            Data = movies,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
        };
    }

    public async Task UpdateDirectorAsync(int id, UpdateDirectorDto command)
    {
        var director = await _dbContext.Directors.FindAsync(id);
        if (director is null)
            throw new ArgumentException($"Director with ID {id} not found.");

        director.Update(command.FirstName, command.LastName, command.DateOfBirth);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteDirectorAsync(int id)
    {
        var director = await _dbContext.Directors.FindAsync(id);

        if (director is null)
            return;

        _dbContext.Directors.Remove(director);
        await _dbContext.SaveChangesAsync();
    }
}

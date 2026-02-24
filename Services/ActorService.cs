using Microsoft.EntityFrameworkCore;
using Movies.Api.Common;

public class ActorService : IActorService
{
    private readonly MovieDbContext _dbContext;

    public ActorService(MovieDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ActorDto> CreateActorAsync(CreateActorDto command)
    {
        var actor = Actor.Create(command.FirstName, command.LastName, command.DateOfBirth);

        await _dbContext.Actors.AddAsync(actor);
        await _dbContext.SaveChangesAsync();

        return new ActorDto(actor.Id, actor.FirstName, actor.LastName, actor.DateOfBirth);
    }

    public async Task<IReadOnlyCollection<ActorDto>> GetAllActorsAsync()
    {
        return await _dbContext.Actors
            .AsNoTracking()
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName)
            .Select(a => new ActorDto(a.Id, a.FirstName, a.LastName, a.DateOfBirth))
            .ToListAsync();
    }

    public async Task<ActorDto?> GetActorByIdAsync(int id)
    {
        return await _dbContext.Actors
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new ActorDto(a.Id, a.FirstName, a.LastName, a.DateOfBirth))
            .FirstOrDefaultAsync();
    }

    public async Task<PagedResponse<MovieSummaryDto>> GetMoviesByActorIdAsync(int id, MovieQueryFilter filter)
    {
        var pageNumber = Math.Max(1, filter.PageNumber ?? 1);
        var pageSize = Math.Clamp(filter.PageSize ?? 10, 1, 50);

        var query = _dbContext.Actors
            .AsNoTracking()
            .Where(a => a.Id == id)
            .SelectMany(a => a.Movies)
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

    public async Task UpdateActorAsync(int id, UpdateActorDto command)
    {
        var actor = await _dbContext.Actors.FindAsync(id);
        if (actor is null)
            throw new ArgumentException($"Actor with ID {id} not found.");

        actor.Update(command.FirstName, command.LastName, command.DateOfBirth);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteActorAsync(int id)
    {
        var actor = await _dbContext.Actors.FindAsync(id);

        if (actor is null)
            return;

        _dbContext.Actors.Remove(actor);
        await _dbContext.SaveChangesAsync();
    }
}

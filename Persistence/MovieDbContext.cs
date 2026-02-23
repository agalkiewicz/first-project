using Microsoft.EntityFrameworkCore;

public class MovieDbContext(DbContextOptions<MovieDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("app");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MovieDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                if (!await context.Set<Movie>().AnyAsync(cancellationToken))
                {
                    var movies = GetSeedMovies();
                    await context.Set<Movie>().AddRangeAsync(movies, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }
            })
            .UseSeeding((context, _) =>
            {
                if (!context.Set<Movie>().Any())
                {
                    var movies = GetSeedMovies();
                    context.Set<Movie>().AddRange(movies);
                    context.SaveChanges();
                }
            });
    }

    private static List<Movie> GetSeedMovies()
    {
        var drama = Category.Create("Drama");
        var crime = Category.Create("Crime");
        var action = Category.Create("Action");
        var fantasy = Category.Create("Fantasy");
        var sciFi = Category.Create("Sci-Fi");
        var animation = Category.Create("Animation");
        var thriller = Category.Create("Thriller");
        var romance = Category.Create("Romance");

        return
        [
            Movie.Create("The Shawshank Redemption", new DateTimeOffset(1994, 9, 23, 0, 0, 0, TimeSpan.Zero), 9.3, [drama]),
            Movie.Create("The Godfather", new DateTimeOffset(1972, 3, 24, 0, 0, 0, TimeSpan.Zero), 9.2, [crime, drama]),
            Movie.Create("The Dark Knight", new DateTimeOffset(2008, 7, 18, 0, 0, 0, TimeSpan.Zero), 9.0, [action, crime]),
            Movie.Create("The Lord of the Rings: The Return of the King", new DateTimeOffset(2003, 12, 17, 0, 0, 0, TimeSpan.Zero), 9.0, [fantasy, action]),
            Movie.Create("Pulp Fiction", new DateTimeOffset(1994, 10, 14, 0, 0, 0, TimeSpan.Zero), 8.9, [crime]),
            Movie.Create("Forrest Gump", new DateTimeOffset(1994, 7, 6, 0, 0, 0, TimeSpan.Zero), 8.8, [drama, romance]),
            Movie.Create("Inception", new DateTimeOffset(2010, 7, 16, 0, 0, 0, TimeSpan.Zero), 8.8, [sciFi, action]),
            Movie.Create("Fight Club", new DateTimeOffset(1999, 10, 15, 0, 0, 0, TimeSpan.Zero), 8.8, [drama]),
            Movie.Create("The Matrix", new DateTimeOffset(1999, 3, 31, 0, 0, 0, TimeSpan.Zero), 8.7, [sciFi, action]),
            Movie.Create("Interstellar", new DateTimeOffset(2014, 11, 7, 0, 0, 0, TimeSpan.Zero), 8.7, [sciFi, drama]),
            Movie.Create("Dune: Part Two", new DateTimeOffset(2024, 3, 1, 0, 0, 0, TimeSpan.Zero), 8.6, [sciFi, action]),
            Movie.Create("Gladiator", new DateTimeOffset(2000, 5, 5, 0, 0, 0, TimeSpan.Zero), 8.5, [action, drama]),
            Movie.Create("The Lion King", new DateTimeOffset(1994, 6, 24, 0, 0, 0, TimeSpan.Zero), 8.5, [animation, drama]),
            Movie.Create("Oppenheimer", new DateTimeOffset(2023, 7, 21, 0, 0, 0, TimeSpan.Zero), 8.5, [drama]),
            Movie.Create("Parasite", new DateTimeOffset(2019, 5, 30, 0, 0, 0, TimeSpan.Zero), 8.5, [thriller, drama]),
            Movie.Create("Jurassic Park", new DateTimeOffset(1993, 6, 11, 0, 0, 0, TimeSpan.Zero), 8.2, [sciFi, action]),
            Movie.Create("Spider-Man: No Way Home", new DateTimeOffset(2021, 12, 17, 0, 0, 0, TimeSpan.Zero), 8.2, [action, fantasy]),
            Movie.Create("The Avengers", new DateTimeOffset(2012, 5, 4, 0, 0, 0, TimeSpan.Zero), 8.0, [action, sciFi]),
            Movie.Create("Titanic", new DateTimeOffset(1997, 12, 19, 0, 0, 0, TimeSpan.Zero), 7.9, [romance, drama]),
            Movie.Create("Everything Everywhere All at Once", new DateTimeOffset(2022, 3, 25, 0, 0, 0, TimeSpan.Zero), 7.8, [sciFi, action]),
        ];
    }
}

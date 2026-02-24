using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class MovieDbContext(DbContextOptions<MovieDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Actor> Actors => Set<Actor>();
    public DbSet<Director> Directors => Set<Director>();

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
                if (!await context.Set<Director>().AnyAsync(cancellationToken))
                {
                    var directors = GetSeedDirectors();
                    await context.Set<Director>().AddRangeAsync(directors, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }

                if (!await context.Set<Actor>().AnyAsync(cancellationToken))
                {
                    var actors = GetSeedActors();
                    await context.Set<Actor>().AddRangeAsync(actors, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }

                if (!await context.Set<Movie>().AnyAsync(cancellationToken))
                {
                    var directors = await context.Set<Director>().ToListAsync(cancellationToken);
                    var actors = await context.Set<Actor>().ToListAsync(cancellationToken);
                    var movies = GetSeedMoviesWithDirectorsAndActors(directors, actors);
                    await context.Set<Movie>().AddRangeAsync(movies, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);
                }
            })
            .UseSeeding((context, _) =>
            {
                if (!context.Set<Director>().Any())
                {
                    var directors = GetSeedDirectors();
                    context.Set<Director>().AddRange(directors);
                    context.SaveChanges();
                }

                if (!context.Set<Actor>().Any())
                {
                    var actors = GetSeedActors();
                    context.Set<Actor>().AddRange(actors);
                    context.SaveChanges();
                }

                if (!context.Set<Movie>().Any())
                {
                    var directors = context.Set<Director>().ToList();
                    var actors = context.Set<Actor>().ToList();
                    var movies = GetSeedMoviesWithDirectorsAndActors(directors, actors);
                    context.Set<Movie>().AddRange(movies);
                    context.SaveChanges();
                }
            });
    }

    private static List<Director> GetSeedDirectors()
    {
        return
        [
            Director.Create("Frank", "Darabont", new DateTimeOffset(1959, 1, 28, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("Francis", "Ford Coppola", new DateTimeOffset(1939, 4, 7, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("Christopher", "Nolan", new DateTimeOffset(1970, 7, 30, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("Peter", "Jackson", new DateTimeOffset(1961, 10, 31, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("Quentin", "Tarantino", new DateTimeOffset(1963, 3, 27, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("Robert", "Zemeckis", new DateTimeOffset(1951, 5, 14, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("David", "Fincher", new DateTimeOffset(1962, 8, 28, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("Lana", "Wachowski", new DateTimeOffset(1965, 6, 21, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("Denis", "Villeneuve", new DateTimeOffset(1967, 10, 3, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("Ridley", "Scott", new DateTimeOffset(1937, 11, 30, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("Steven", "Spielberg", new DateTimeOffset(1946, 12, 18, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("James", "Cameron", new DateTimeOffset(1954, 8, 16, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("Bong", "Joon-ho", new DateTimeOffset(1969, 9, 14, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("Jon", "Watts", new DateTimeOffset(1981, 6, 28, 0, 0, 0, TimeSpan.Zero)),
            Director.Create("Joss", "Whedon", new DateTimeOffset(1964, 6, 23, 0, 0, 0, TimeSpan.Zero)),
        ];
    }

    private static List<Actor> GetSeedActors()
    {
        return
        [
            Actor.Create("Morgan", "Freeman", new DateTimeOffset(1937, 6, 1, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Tim", "Robbins", new DateTimeOffset(1958, 10, 16, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Marlon", "Brando", new DateTimeOffset(1924, 4, 3, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Al", "Pacino", new DateTimeOffset(1940, 4, 25, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Christian", "Bale", new DateTimeOffset(1974, 1, 30, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Heath", "Ledger", new DateTimeOffset(1979, 4, 4, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Elijah", "Wood", new DateTimeOffset(1981, 1, 28, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Ian", "McKellen", new DateTimeOffset(1939, 5, 25, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("John", "Travolta", new DateTimeOffset(1954, 2, 18, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Samuel L.", "Jackson", new DateTimeOffset(1948, 12, 21, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Tom", "Hanks", new DateTimeOffset(1956, 7, 9, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Leonardo", "DiCaprio", new DateTimeOffset(1974, 11, 11, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Brad", "Pitt", new DateTimeOffset(1963, 12, 18, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Keanu", "Reeves", new DateTimeOffset(1964, 9, 2, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Matthew", "McConaughey", new DateTimeOffset(1969, 11, 4, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Timothée", "Chalamet", new DateTimeOffset(1995, 12, 27, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Russell", "Crowe", new DateTimeOffset(1964, 4, 7, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Cillian", "Murphy", new DateTimeOffset(1976, 5, 25, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Song", "Kang-ho", new DateTimeOffset(1967, 1, 17, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Tom", "Holland", new DateTimeOffset(1996, 6, 1, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Robert", "Downey Jr.", new DateTimeOffset(1965, 4, 4, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Michelle", "Yeoh", new DateTimeOffset(1962, 8, 6, 0, 0, 0, TimeSpan.Zero)),
            Actor.Create("Kate", "Winslet", new DateTimeOffset(1975, 10, 5, 0, 0, 0, TimeSpan.Zero)),
        ];
    }

    private static List<Movie> GetSeedMoviesWithDirectorsAndActors(List<Director> directors, List<Actor> actors)
    {
        var drama = Category.Create("Drama");
        var crime = Category.Create("Crime");
        var action = Category.Create("Action");
        var fantasy = Category.Create("Fantasy");
        var sciFi = Category.Create("Sci-Fi");
        var animation = Category.Create("Animation");
        var thriller = Category.Create("Thriller");
        var romance = Category.Create("Romance");

        // Find directors by last name
        var darabont = directors.First(d => d.LastName == "Darabont");
        var coppola = directors.First(d => d.LastName == "Ford Coppola");
        var nolan = directors.First(d => d.LastName == "Nolan");
        var jackson = directors.First(d => d.LastName == "Jackson");
        var tarantino = directors.First(d => d.LastName == "Tarantino");
        var zemeckis = directors.First(d => d.LastName == "Zemeckis");
        var fincher = directors.First(d => d.LastName == "Fincher");
        var wachowski = directors.First(d => d.LastName == "Wachowski");
        var villeneuve = directors.First(d => d.LastName == "Villeneuve");
        var scott = directors.First(d => d.LastName == "Scott");
        var spielberg = directors.First(d => d.LastName == "Spielberg");
        var cameron = directors.First(d => d.LastName == "Cameron");
        var bong = directors.First(d => d.LastName == "Joon-ho");
        var watts = directors.First(d => d.LastName == "Watts");
        var whedon = directors.First(d => d.LastName == "Whedon");

        // Find actors by last name
        var morganFreeman = actors.First(a => a.LastName == "Freeman");
        var timRobbins = actors.First(a => a.LastName == "Robbins");
        var marlonBrando = actors.First(a => a.LastName == "Brando");
        var alPacino = actors.First(a => a.LastName == "Pacino");
        var christianBale = actors.First(a => a.LastName == "Bale");
        var heathLedger = actors.First(a => a.LastName == "Ledger");
        var elijahWood = actors.First(a => a.LastName == "Wood");
        var ianMcKellen = actors.First(a => a.LastName == "McKellen");
        var johnTravolta = actors.First(a => a.LastName == "Travolta");
        var samuelJackson = actors.First(a => a.FirstName == "Samuel L.");
        var tomHanks = actors.First(a => a.LastName == "Hanks");
        var leonardoDiCaprio = actors.First(a => a.LastName == "DiCaprio");
        var bradPitt = actors.First(a => a.LastName == "Pitt");
        var keanuReeves = actors.First(a => a.LastName == "Reeves");
        var matthewMcConaughey = actors.First(a => a.LastName == "McConaughey");
        var timotheeChalamet = actors.First(a => a.LastName == "Chalamet");
        var russellCrowe = actors.First(a => a.LastName == "Crowe");
        var cillianMurphy = actors.First(a => a.LastName == "Murphy");
        var songKangho = actors.First(a => a.LastName == "Kang-ho");
        var tomHolland = actors.First(a => a.LastName == "Holland");
        var robertDowneyJr = actors.First(a => a.LastName == "Downey Jr.");
        var michelleYeoh = actors.First(a => a.LastName == "Yeoh");
        var kateWinslet = actors.First(a => a.LastName == "Winslet");

        var shawshank = Movie.Create("The Shawshank Redemption", new DateTimeOffset(1994, 9, 23, 0, 0, 0, TimeSpan.Zero), 9.3, [drama]);
        shawshank.SetDirector(darabont);
        shawshank.SetActors([morganFreeman, timRobbins]);

        var godfather = Movie.Create("The Godfather", new DateTimeOffset(1972, 3, 24, 0, 0, 0, TimeSpan.Zero), 9.2, [crime, drama]);
        godfather.SetDirector(coppola);
        godfather.SetActors([marlonBrando, alPacino]);

        var darkKnight = Movie.Create("The Dark Knight", new DateTimeOffset(2008, 7, 18, 0, 0, 0, TimeSpan.Zero), 9.0, [action, crime]);
        darkKnight.SetDirector(nolan);
        darkKnight.SetActors([christianBale, heathLedger, morganFreeman, cillianMurphy]);

        var lotr = Movie.Create("The Lord of the Rings: The Return of the King", new DateTimeOffset(2003, 12, 17, 0, 0, 0, TimeSpan.Zero), 9.0, [fantasy, action]);
        lotr.SetDirector(jackson);
        lotr.SetActors([elijahWood, ianMcKellen]);

        var pulpFiction = Movie.Create("Pulp Fiction", new DateTimeOffset(1994, 10, 14, 0, 0, 0, TimeSpan.Zero), 8.9, [crime]);
        pulpFiction.SetDirector(tarantino);
        pulpFiction.SetActors([johnTravolta, samuelJackson]);

        var forrestGump = Movie.Create("Forrest Gump", new DateTimeOffset(1994, 7, 6, 0, 0, 0, TimeSpan.Zero), 8.8, [drama, romance]);
        forrestGump.SetDirector(zemeckis);
        forrestGump.SetActors([tomHanks]);

        var inception = Movie.Create("Inception", new DateTimeOffset(2010, 7, 16, 0, 0, 0, TimeSpan.Zero), 8.8, [sciFi, action]);
        inception.SetDirector(nolan);
        inception.SetActors([leonardoDiCaprio, cillianMurphy]);

        var fightClub = Movie.Create("Fight Club", new DateTimeOffset(1999, 10, 15, 0, 0, 0, TimeSpan.Zero), 8.8, [drama]);
        fightClub.SetDirector(fincher);
        fightClub.SetActors([bradPitt]);

        var matrix = Movie.Create("The Matrix", new DateTimeOffset(1999, 3, 31, 0, 0, 0, TimeSpan.Zero), 8.7, [sciFi, action]);
        matrix.SetDirector(wachowski);
        matrix.SetActors([keanuReeves]);

        var interstellar = Movie.Create("Interstellar", new DateTimeOffset(2014, 11, 7, 0, 0, 0, TimeSpan.Zero), 8.7, [sciFi, drama]);
        interstellar.SetDirector(nolan);
        interstellar.SetActors([matthewMcConaughey]);

        var dune2 = Movie.Create("Dune: Part Two", new DateTimeOffset(2024, 3, 1, 0, 0, 0, TimeSpan.Zero), 8.6, [sciFi, action]);
        dune2.SetDirector(villeneuve);
        dune2.SetActors([timotheeChalamet]);

        var gladiator = Movie.Create("Gladiator", new DateTimeOffset(2000, 5, 5, 0, 0, 0, TimeSpan.Zero), 8.5, [action, drama]);
        gladiator.SetDirector(scott);
        gladiator.SetActors([russellCrowe]);

        var lionKing = Movie.Create("The Lion King", new DateTimeOffset(1994, 6, 24, 0, 0, 0, TimeSpan.Zero), 8.5, [animation, drama]);
        // Animation - no live actors

        var oppenheimer = Movie.Create("Oppenheimer", new DateTimeOffset(2023, 7, 21, 0, 0, 0, TimeSpan.Zero), 8.5, [drama]);
        oppenheimer.SetDirector(nolan);
        oppenheimer.SetActors([cillianMurphy, robertDowneyJr]);

        var parasite = Movie.Create("Parasite", new DateTimeOffset(2019, 5, 30, 0, 0, 0, TimeSpan.Zero), 8.5, [thriller, drama]);
        parasite.SetDirector(bong);
        parasite.SetActors([songKangho]);

        var jurassicPark = Movie.Create("Jurassic Park", new DateTimeOffset(1993, 6, 11, 0, 0, 0, TimeSpan.Zero), 8.2, [sciFi, action]);
        jurassicPark.SetDirector(spielberg);
        jurassicPark.SetActors([samuelJackson]);

        var spiderMan = Movie.Create("Spider-Man: No Way Home", new DateTimeOffset(2021, 12, 17, 0, 0, 0, TimeSpan.Zero), 8.2, [action, fantasy]);
        spiderMan.SetDirector(watts);
        spiderMan.SetActors([tomHolland]);

        var avengers = Movie.Create("The Avengers", new DateTimeOffset(2012, 5, 4, 0, 0, 0, TimeSpan.Zero), 8.0, [action, sciFi]);
        avengers.SetDirector(whedon);
        avengers.SetActors([robertDowneyJr, samuelJackson]);

        var titanic = Movie.Create("Titanic", new DateTimeOffset(1997, 12, 19, 0, 0, 0, TimeSpan.Zero), 7.9, [romance, drama]);
        titanic.SetDirector(cameron);
        titanic.SetActors([leonardoDiCaprio, kateWinslet]);

        var eeaao = Movie.Create("Everything Everywhere All at Once", new DateTimeOffset(2022, 3, 25, 0, 0, 0, TimeSpan.Zero), 7.8, [sciFi, action]);
        eeaao.SetActors([michelleYeoh]);

        return [shawshank, godfather, darkKnight, lotr, pulpFiction, forrestGump, inception, fightClub, matrix, interstellar, dune2, gladiator, lionKing, oppenheimer, parasite, jurassicPark, spiderMan, avengers, titanic, eeaao];
    }

    public static async Task SeedEssentialsAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        //Seed Roles
        await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.Administrator.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.Moderator.ToString()));
        await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.User.ToString()));

        //Seed Default User
        var defaultUser = new ApplicationUser { UserName = Authorization.default_username, Email = Authorization.default_email, EmailConfirmed = true, PhoneNumberConfirmed = true };

        if (userManager.Users.All(u => u.Id != defaultUser.Id))
        {
            await userManager.CreateAsync(defaultUser, Authorization.default_password);
            await userManager.AddToRoleAsync(defaultUser, Authorization.default_role.ToString());
        }
    }
}

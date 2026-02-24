public sealed class Movie : EntityBase
{
    public string Title { get; private set; }
    public DateTimeOffset ReleaseDate { get; private set; }
    public double Rating { get; private set; }
    public List<Category> Categories { get; private set; } = [];
    public List<Actor> Actors { get; private set; } = [];
    public int? DirectorId { get; private set; }
    public Director? Director { get; private set; }

    // Private constructor for ORM frameworks
    private Movie()
    {
        Title = string.Empty;
    }

    private Movie(string title, DateTimeOffset releaseDate, double rating, List<Category> categories)
    {
        Title = title;
        ReleaseDate = releaseDate;
        Rating = rating;
        SetCategories(categories);
    }

    public static Movie Create(string title, DateTimeOffset releaseDate, double rating, List<Category> categories)
    {
        return new Movie(title, releaseDate, rating, categories);
    }

    public void Update(string title, DateTimeOffset releaseDate, double rating, List<Category> categories)
    {
        Title = title;
        ReleaseDate = releaseDate;
        Rating = rating;
        SetCategories(categories);

        UpdateLastModified();
    }

    public void SetDirector(Director? director)
    {
        Director = director;
        DirectorId = director?.Id;
        UpdateLastModified();
    }

    public void SetActors(List<Actor> actors)
    {
        Actors.Clear();
        Actors.AddRange(actors.DistinctBy(a => a.Id));
        UpdateLastModified();
    }

    private void SetCategories(List<Category> categories)
    {
        Categories.Clear();
        Categories.AddRange(categories.DistinctBy(c => c.Id));
    }
}

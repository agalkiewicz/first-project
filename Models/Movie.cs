public sealed class Movie : EntityBase
{
    public string Title { get; private set; }
    public DateTimeOffset ReleaseDate { get; private set; }
    public double Rating { get; private set; }
    public List<Category> Categories { get; private set; } = new List<Category>();

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

    private void SetCategories(List<Category> categories)
    {
        Categories.Clear();
        Categories.AddRange(categories.DistinctBy(c => c.Id));
    }
}

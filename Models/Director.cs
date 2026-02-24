public sealed class Director : PersonBase
{
    public List<Movie> Movies { get; private set; } = [];

    private Director() : base()
    {
    }

    private Director(string firstName, string lastName, DateTimeOffset dateOfBirth)
        : base(firstName, lastName, dateOfBirth)
    {
    }

    public static Director Create(string firstName, string lastName, DateTimeOffset dateOfBirth)
    {
        return new Director(firstName, lastName, dateOfBirth);
    }
}

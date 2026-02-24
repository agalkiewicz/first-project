public sealed class Actor : PersonBase
{
    public List<Movie> Movies { get; private set; } = [];

    private Actor() : base()
    {
    }

    private Actor(string firstName, string lastName, DateTimeOffset dateOfBirth)
        : base(firstName, lastName, dateOfBirth)
    {
    }

    public static Actor Create(string firstName, string lastName, DateTimeOffset dateOfBirth)
    {
        return new Actor(firstName, lastName, dateOfBirth);
    }
}

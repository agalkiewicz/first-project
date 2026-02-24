public abstract class PersonBase : EntityBase
{
    public string FirstName { get; protected set; }
    public string LastName { get; protected set; }
    public DateTimeOffset DateOfBirth { get; protected set; }

    protected PersonBase()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
    }

    protected PersonBase(string firstName, string lastName, DateTimeOffset dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
    }

    public void Update(string firstName, string lastName, DateTimeOffset dateOfBirth)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        UpdateLastModified();
    }
}

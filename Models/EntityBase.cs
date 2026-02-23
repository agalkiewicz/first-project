public abstract class EntityBase
{
    public int Id { get; private set; }
    public DateTimeOffset Created { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset LastModified { get; private set; } = DateTimeOffset.UtcNow;
    public void UpdateLastModified()
    {
        LastModified = DateTimeOffset.UtcNow;
    }
}

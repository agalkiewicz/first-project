public sealed class Category : EntityBase
{
	public string Name { get; private set; }
	public ICollection<Movie> Movies { get; private set; } = new List<Movie>();

	private Category()
	{
		Name = string.Empty;
	}

	private Category(string name)
	{
		Name = name;
	}

	public static Category Create(string name)
	{
		return new Category(name);
	}

	public void Update(string name)
	{
		Name = name;
		UpdateLastModified();
	}
}

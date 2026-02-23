using System.ComponentModel.DataAnnotations;

public record CreateMovieDto(
	[property: Required, StringLength(200)] string Title,
	[property: Required] DateTimeOffset ReleaseDate,
	[property: Range(0, 10)] double Rating,
	IReadOnlyCollection<int>? CategoryIds = null
);

using System.ComponentModel.DataAnnotations;

public record UpdateMovieDto(
	[property: Required, StringLength(200)] string Title,
	[property: Required, StringLength(100)] string Genre,
	[property: Required] DateTimeOffset ReleaseDate,
	[property: Range(0, 10)] double Rating
);

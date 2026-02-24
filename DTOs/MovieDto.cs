public record MovieDto(int Id, string Title, DateTimeOffset ReleaseDate, double Rating, IReadOnlyCollection<CategoryDto> Categories, DirectorSummaryDto? Director);

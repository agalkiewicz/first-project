namespace Movies.Api.Common;

public class MovieQueryFilter
{
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public string? SortBy { get; set; }
    public string? Search { get; set; }
}

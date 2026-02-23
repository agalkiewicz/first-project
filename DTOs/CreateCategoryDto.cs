using System.ComponentModel.DataAnnotations;

public record CreateCategoryDto(
    [property: Required, StringLength(100)] string Name
);

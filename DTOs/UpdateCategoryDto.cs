using System.ComponentModel.DataAnnotations;

public record UpdateCategoryDto(
    [property: Required, StringLength(100)] string Name
);

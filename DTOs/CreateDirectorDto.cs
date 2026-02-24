using System.ComponentModel.DataAnnotations;

public record CreateDirectorDto(
    [property: Required, StringLength(100)] string FirstName,
    [property: Required, StringLength(100)] string LastName,
    [property: Required] DateTimeOffset DateOfBirth
);

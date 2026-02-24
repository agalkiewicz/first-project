using System.ComponentModel.DataAnnotations;

public class UserRegistrationRequest
{
    [Required]
    public required string FirstName { get; set; }

    [Required]
    public required string LastName { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public required string ConfirmPassword { get; set; }
}

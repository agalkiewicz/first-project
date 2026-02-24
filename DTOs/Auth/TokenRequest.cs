using System.ComponentModel.DataAnnotations;

public class TokenRequest
{
    [Required]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
}

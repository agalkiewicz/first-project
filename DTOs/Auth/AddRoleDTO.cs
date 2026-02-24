using System.ComponentModel.DataAnnotations;

public class AddRoleDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
    [Required]
    public required string Role { get; set; }
}
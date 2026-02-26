using System.ComponentModel.DataAnnotations;

namespace Wonga.Api.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string FirstName { get; set; } = default!;

    [Required]
    public string LastName { get; set; } = default!;

    [Required]
    public string Email { get; set; } = default!;

    [Required]
    public string PasswordHash { get; set; } = default!;
}

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

    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public string? ResetCode { get; set; }
public DateTime? ResetCodeExpiry { get; set; }
}

// ForgotPasswordRequest.cs
using System.ComponentModel.DataAnnotations;
namespace Wonga.Api.DTOs
{
public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}

// ResetPasswordRequest.cs
public class ResetPasswordRequest
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = string.Empty;
}}
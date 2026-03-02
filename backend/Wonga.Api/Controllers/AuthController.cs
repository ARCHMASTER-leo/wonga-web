using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Wonga.Api.Data;
using Wonga.Api.DTOs;
using Wonga.Api.Services;
using Wonga.Api.Services.Interfaces;

namespace Wonga.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwt;
        private readonly IAuthService _authService;

        public AuthController(AppDbContext context, JwtService jwt, IAuthService authService)
        {
            _context = context;
            _jwt = jwt;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _authService.RegisterAsync(dto);
                return StatusCode(201, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginRequest dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    try
    {
        var result = await _authService.LoginAsync(dto);
        return Ok(new { result.Token });
    }
    catch (UnauthorizedAccessException ex)
    {
        return Unauthorized(new { message = ex.Message });
    }
}

[Authorize]
[HttpPost("change-password")]
public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var user = await _context.Users.FindAsync(Guid.Parse(userId!));

    if (user == null)
        return NotFound();

    if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
        return Unauthorized(new { message = "Current password is incorrect." });

    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Password changed successfully." });
}

[Authorize]
[HttpPut("update")]
public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var user = await _context.Users.FindAsync(Guid.Parse(userId!));

    if (user == null)
        return NotFound();

    user.FirstName = dto.FirstName;
    user.LastName = dto.LastName;
    user.Email = dto.Email;
    user.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return Ok(new
    {
        user.Id,
        user.FirstName,
        user.LastName,
        user.Email,
        user.CreatedAt,
        user.UpdatedAt
    });
}

[HttpPost("forgot-password")]
public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest dto)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

    if (user == null)
        return Ok(new { message = "If that email exists, a code was generated." });

    var code = new Random().Next(100000, 999999).ToString();
    user.ResetCode = code;
    user.ResetCodeExpiry = DateTime.UtcNow.AddMinutes(15);
    await _context.SaveChangesAsync();

    // DEV ONLY — return code in response
    return Ok(new { message = "Reset code generated.", code });
}

[HttpPost("reset-password")]
public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest dto)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

    if (user == null || user.ResetCode != dto.Code || user.ResetCodeExpiry < DateTime.UtcNow)
        return BadRequest(new { message = "Invalid or expired reset code." });

    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
    user.ResetCode = null;
    user.ResetCodeExpiry = null;
    await _context.SaveChangesAsync();

    return Ok(new { message = "Password reset successfully." });
}

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid token." });

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found." });

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.CreatedAt
            });
        }
    }
}
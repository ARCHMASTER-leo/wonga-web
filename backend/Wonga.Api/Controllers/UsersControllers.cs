using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Wonga.Api.Data;
using Wonga.Api.DTOs;

namespace Wonga.Api.Controllers;
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _context.Users.FindAsync(Guid.Parse(userId!));

        return Ok(new UserResponse
        {
            Id = user!.Id,
            Email = user.Email
        });
    }
}
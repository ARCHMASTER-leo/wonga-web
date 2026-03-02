using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Wonga.Api.Data;
using Wonga.Api.DTOs;
using Wonga.Api.Models;
using Wonga.Api.Services.Interfaces;

namespace Wonga.Api.Services;

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

public async Task<AuthResponse> LoginAsync(LoginRequest request)
{
    var user = await _context.Users
        .FirstOrDefaultAsync(x => x.Email == request.Email);

    // This must throw — not return null or BadRequest
    if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        throw new UnauthorizedAccessException("Invalid credentials.");

    var token = GenerateJwt(user);
    return new AuthResponse { Token = token };
}
   public async Task<UserResponse> RegisterAsync(RegisterRequest request)
{
    var existing = await _context.Users
        .FirstOrDefaultAsync(x => x.Email == request.Email);

    if (existing != null)
        throw new InvalidOperationException("Email already in use.");

    var user = new User
    {
        FirstName    = request.FirstName,
        LastName     = request.LastName,
        Email        = request.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
    };

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return new UserResponse
    {
        Id        = user.Id,
        FirstName = user.FirstName,
        LastName  = user.LastName,
        Email     = user.Email,
        CreatedAt = user.CreatedAt
    };
}

    // public Task<UserResponse> RegisterAsync(RegisterRequest request)
    // {
    //     throw new NotImplementedException();
    // }

    private string GenerateJwt(User user)
{
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email)
    };

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
    );

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

}
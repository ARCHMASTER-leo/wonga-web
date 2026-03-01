using Microsoft.EntityFrameworkCore;
using Wonga.Api.Data;
using Wonga.Api.DTOs;
using Wonga.Api.Models;
using Wonga.Api.Services.Interfaces;

namespace Wonga.Api.Services;

public class NewBaseType
{
    public Task<UserResponse> RegisterAsync(RegisterRequest request)
    {
        throw new NotImplementedException();
    }
}

public class UserService(AppDbContext context) : NewBaseType, IUserService
{
    private readonly AppDbContext _context = context;

    public new async Task<UserResponse> RegisterAsync(RegisterRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = request.Password // we'll hash later
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email
        };
    }

    public async Task<List<UserResponse>> GetAllAsync()
    {
        return await _context.Users
            .Select(u => new UserResponse
            {
                Id = u.Id,
                Email = u.Email
            })
            .ToListAsync();
    }
}
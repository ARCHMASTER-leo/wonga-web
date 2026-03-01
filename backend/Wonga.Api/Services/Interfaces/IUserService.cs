
using Wonga.Api.DTOs;

namespace Wonga.Api.Services.Interfaces;

public interface IUserService
{
    Task<UserResponse> RegisterAsync(RegisterRequest request);
    Task<List<UserResponse>> GetAllAsync();
}
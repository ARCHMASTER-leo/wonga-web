using Wonga.Api.DTOs;

namespace Wonga.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserResponse> RegisterAsync(RegisterRequest request); // ← must return Task<UserResponse>
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
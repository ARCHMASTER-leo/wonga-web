using Moq;
using Microsoft.AspNetCore.Mvc;
using Wonga.Api.Controllers;
using Wonga.Api.DTOs;
using Wonga.Api.Services.Interfaces;
using Wonga.Api.Data;
using Wonga.Api.Services;
using Xunit;

namespace Wonga.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();

            // Pass null for AppDbContext and JwtService since we're mocking IAuthService
            _controller = new AuthController(
                context: null!,
                jwt: null!,
                authService: _authServiceMock.Object
            );
        }

        [Fact]
        public async Task Register_ReturnsCreated_WhenSuccessful()
        {
            // Arrange
            var request = new RegisterRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password123"
            };

            var userResponse = new UserResponse
            {
                Id = Guid.NewGuid(),
                Email = request.Email
            };

            // Fix: return type is Task<UserResponse>, not Task
            _authServiceMock
                .Setup(x => x.RegisterAsync(request))
                .ReturnsAsync(userResponse);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, statusResult.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenInvalidCredentials()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "john@example.com",
                Password = "wrongpassword"
            };

            _authServiceMock
                .Setup(x => x.LoginAsync(request))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid email or password."));

            // Act
            var result = await _controller.Login(request);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
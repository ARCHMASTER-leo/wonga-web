using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wonga.Api.Data;
using Wonga.Api.DTOs;
using Xunit;

namespace Wonga.Tests.Integration
{
    public class AuthIntegrationTests : IClassFixture<AuthWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthIntegrationTests(AuthWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        // ──────────────────────────────────────────────
        // REGISTER
        // ──────────────────────────────────────────────

        [Fact]
        public async Task Register_Should_Return_201_When_Valid()
        {
            var request = new RegisterRequest
            {
                FirstName = "John",
                LastName  = "Doe",
                Email     = $"register_{Guid.NewGuid()}@test.com",
                Password  = "password123"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<UserResponse>();
            Assert.NotNull(body);
            Assert.Equal(request.Email, body!.Email);
        }

        [Fact]
        public async Task Register_Should_Return_409_When_Email_Already_Exists()
        {
            var request = new RegisterRequest
            {
                FirstName = "Jane",
                LastName  = "Doe",
                Email     = $"duplicate_{Guid.NewGuid()}@test.com",
                Password  = "password123"
            };

            await _client.PostAsJsonAsync("/api/auth/register", request);
            var response = await _client.PostAsJsonAsync("/api/auth/register", request);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Register_Should_Return_400_When_Email_Is_Invalid()
        {
            var request = new RegisterRequest
            {
                FirstName = "Bad",
                LastName  = "Request",
                Email     = "not-an-email",
                Password  = "password123"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", request);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ──────────────────────────────────────────────
        // LOGIN
        // ──────────────────────────────────────────────

        [Fact]
        public async Task Login_Should_Return_200_And_Token_When_Valid()
        {
            var email    = $"login_{Guid.NewGuid()}@test.com";
            var password = "password123";

            await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                FirstName = "Login",
                LastName  = "User",
                Email     = email,
                Password  = password
            });

            var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email    = email,
                Password = password
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(body);
            Assert.False(string.IsNullOrWhiteSpace(body!.Token));
        }

        [Fact]
        public async Task Login_Should_Return_401_When_Password_Is_Wrong()
        {
            var email = $"wrongpass_{Guid.NewGuid()}@test.com";

            await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
            {
                FirstName = "Wrong",
                LastName  = "Pass",
                Email     = email,
                Password  = "correctpassword"
            });

            var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email    = email,
                Password = "wrongpassword"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_Should_Return_401_When_User_Does_Not_Exist()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email    = "ghost@nowhere.com",
                Password = "doesntmatter"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }

    // ──────────────────────────────────────────────
    // CUSTOM FACTORY — swaps Postgres for InMemory DB
    // ──────────────────────────────────────────────
public class AuthWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"]      = "super-secret-test-key-that-is-long-enough-32chars!",
                ["Jwt:Issuer"]   = "TestIssuer",
                ["Jwt:Audience"] = "TestAudience"
            });
        });

  builder.ConfigureServices(services =>
{
    var descriptor = services.SingleOrDefault(
        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
    if (descriptor != null)
        services.Remove(descriptor);

    services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TestDb")); 
});

        builder.UseEnvironment("Testing");
    }
}
}
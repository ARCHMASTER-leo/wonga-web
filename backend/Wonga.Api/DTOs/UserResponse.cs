namespace Wonga.Api.DTOs
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; internal set; } = string.Empty;
        public string LastName { get; internal set; } = string.Empty;
    }
}
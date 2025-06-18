using Microsoft.AspNetCore.Identity;
using OVOVAX.Core.Entities.Identity;
using OVOVAX.Core.DTOs.Auth;
using System.ComponentModel.DataAnnotations;

namespace OVOVAX.Core.Services.Contract
{
    public interface IAuthService
    {
        // Core authentication methods
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<ForgotPasswordResponse> ForgotPasswordAsync(string email);     
           Task<AuthResponse> ValidateTokenAsync(string token);
        Task<bool> CheckEmailExistsAsync(string email);
        Task<string> CreateTokenAsync(AppUser user);
        PasswordValidationResult ValidatePassword(string password);
    }

    // DTOs defined in the same file
    public class AuthResponse
    {        public bool Success { get; set; }
        public string? Token { get; set; }
        public UserDto? User { get; set; }
        public string? Error { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class ForgotPasswordResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? Error { get; set; }
    }

    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime? LastLogin { get; set; }
    }

    public class ValidatePasswordRequest
    {
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

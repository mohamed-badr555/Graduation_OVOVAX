using System.ComponentModel.DataAnnotations;

namespace OVOVAX.Core.DTOs.Auth
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}

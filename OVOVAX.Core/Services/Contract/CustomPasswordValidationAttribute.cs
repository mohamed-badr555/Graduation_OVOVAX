using System.ComponentModel.DataAnnotations;

namespace OVOVAX.Core.Services.Contract
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CustomPasswordValidationAttribute : ValidationAttribute
    {
        public CustomPasswordValidationAttribute() : base("Password does not meet the required criteria.")
        {
        }

        public override bool IsValid(object? value)
        {
            if (value is not string password)
                return false;

            var result = PasswordValidator.ValidatePassword(password);
            if (!result.IsValid)
            {
                ErrorMessage = GetPasswordErrorMessage(result.Requirements);
            }
            
            return result.IsValid;
        }

        private string GetPasswordErrorMessage(PasswordRequirements requirements)
        {
            var errors = new List<string>();
            
            if (!requirements.Length) errors.Add("at least 8 characters");
            if (!requirements.Uppercase) errors.Add("one uppercase letter");
            if (!requirements.Lowercase) errors.Add("one lowercase letter");
            if (!requirements.Number) errors.Add("one number");
            if (!requirements.Special) errors.Add("one special character (!@#$%^&*(),.?\":{}|<>)");

            return $"Password must contain: {string.Join(", ", errors)}";
        }
    }
}

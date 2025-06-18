namespace OVOVAX.Core.Services.Contract
{
    public static class PasswordValidator
    {
        public static PasswordValidationResult ValidatePassword(string password)
        {
            var result = new PasswordValidationResult
            {
                IsValid = true,
                Requirements = new PasswordRequirements()
            };

            if (string.IsNullOrEmpty(password))
            {
                result.IsValid = false;
                return result;
            }

            // Apply your exact requirements
            result.Requirements.Length = password.Length >= 8;
            result.Requirements.Uppercase = password.Any(char.IsUpper);
            result.Requirements.Lowercase = password.Any(char.IsLower);
            result.Requirements.Number = password.Any(char.IsDigit);
            result.Requirements.Special = password.Any(ch => "!@#$%^&*(),.?\":{}|<>".Contains(ch));

            result.IsValid = result.Requirements.Length && 
                            result.Requirements.Uppercase && 
                            result.Requirements.Lowercase && 
                            result.Requirements.Number && 
                            result.Requirements.Special;

            return result;
        }
    }
}

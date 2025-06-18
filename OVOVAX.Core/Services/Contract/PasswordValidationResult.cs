namespace OVOVAX.Core.Services.Contract
{
    public class PasswordValidationResult
    {
        public bool IsValid { get; set; }
        public PasswordRequirements Requirements { get; set; } = new();
    }
}

namespace OVOVAX.Core.Services.Contract
{
    public class PasswordRequirements
    {
        public bool Length { get; set; } = false;
        public bool Uppercase { get; set; } = false;
        public bool Lowercase { get; set; } = false;
        public bool Number { get; set; } = false;
        public bool Special { get; set; } = false;
    }
}

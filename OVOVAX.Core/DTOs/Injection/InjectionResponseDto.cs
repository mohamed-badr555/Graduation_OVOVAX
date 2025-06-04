namespace OVOVAX.Core.DTOs.Injection
{
    public class InjectionResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? SessionId { get; set; }
    }
}

namespace OVOVAX.API.DTOs.Injection
{
    public class InjectionResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? OperationId { get; set; }
        public object? Data { get; set; }
    }
}

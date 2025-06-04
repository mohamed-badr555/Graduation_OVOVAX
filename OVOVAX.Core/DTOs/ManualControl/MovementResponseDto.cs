namespace OVOVAX.Core.DTOs.ManualControl
{
    public class MovementResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? MovementId { get; set; }
    }
}

namespace OVOVAX.Core.DTOs.ManualControl
{
    public class MovementHistoryDto
    {
        public string Timestamp { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public int Speed { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

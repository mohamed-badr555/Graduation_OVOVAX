namespace OVOVAX.Core.DTOs.Injection
{
    public class InjectionHistoryDto
    {
        public string Time { get; set; } = string.Empty;
        public int EggNumber { get; set; }
        public double Volume { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

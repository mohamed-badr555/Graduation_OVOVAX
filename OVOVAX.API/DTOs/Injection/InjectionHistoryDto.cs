namespace OVOVAX.API.DTOs.Injection
{
    public class InjectionHistoryDto
    {
        public int Id { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string? EndTime { get; set; }
        public int EggNumber { get; set; }
        public double Volume { get; set; }
        public double RangeFrom { get; set; }
        public double RangeTo { get; set; }
        public double Step { get; set; }
        public string Status { get; set; } = string.Empty;
        //public string? Notes { get; set; }
    }
}

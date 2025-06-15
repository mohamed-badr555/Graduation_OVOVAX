namespace OVOVAX.API.DTOs.Scanner
{
    public class ScanResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? ScanId { get; set; }
        public string? Status { get; set; } // Status of the scan
        public double[]? Readings { get; set; } // Array of sensor readings collected during scan
        public int? ReadingCount { get; set; } // Number of readings collected
    }
}

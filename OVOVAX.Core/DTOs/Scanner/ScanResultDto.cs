using System;

namespace OVOVAX.Core.DTOs.Scanner
{
    public class ScanResultDto
    {
        public int Id { get; set; }
        public DateTime ScanTime { get; set; }
        public double DepthMeasurement { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

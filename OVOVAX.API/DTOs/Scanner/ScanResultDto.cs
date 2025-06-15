using System;
using System.Collections.Generic;

namespace OVOVAX.API.DTOs.Scanner
{
    public class ScanResultDto
    {
        public int Id { get; set; }
        public DateTime ScanTime { get; set; }
        public List<double> SensorReadings { get; set; } = new List<double>();
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public int ReadingCount => SensorReadings.Count;
    }
}

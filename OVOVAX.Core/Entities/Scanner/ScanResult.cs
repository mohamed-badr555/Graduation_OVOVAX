using System;
using System.Collections.Generic;

namespace OVOVAX.Core.Entities.Scanner
{
    public class ScanResult : BaseEntity
    {
        public DateTime ScanTime { get; set; }
        
        public List<double> SensorReadings { get; set; } = new List<double>(); // Array of sensor readings in mm
        public ScanStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public enum ScanStatus
    {
        Success,
        Failed,
        InProgress
    }
}

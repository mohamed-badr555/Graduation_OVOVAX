using System;
using System.Collections.Generic;
using OVOVAX.Core.Entities.Identity;

namespace OVOVAX.Core.Entities.Scanner
{
    public class ScanResult : BaseEntity
    {
        public DateTime ScanTime { get; set; }
        
        public List<double> SensorReadings { get; set; } = new List<double>(); // Array of sensor readings in mm
        public ScanStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        
        // User relationship
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
    }

    public enum ScanStatus
    {
        Success,
        Failed,
        InProgress
    }
}

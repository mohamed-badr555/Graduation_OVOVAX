using System;

namespace OVOVAX.Core.Entities.Scanner
{
    public class ScanResult : BaseEntity
    {
        public DateTime ScanTime { get; set; }
        public double DepthMeasurement { get; set; } // in mm
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

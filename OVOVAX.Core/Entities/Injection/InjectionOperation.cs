using System;

namespace OVOVAX.Core.Entities.Injection
{
    public class InjectionOperation : BaseEntity
    {
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double RangeOfInfraredFrom { get; set; } // in mm
        public double RangeOfInfraredTo { get; set; } // in mm
        public double StepOfInjection { get; set; } // in mm
        public double VolumeOfLiquid { get; set; } // in ml
        public int NumberOfElements { get; set; }
        public InjectionStatus Status { get; set; }
        //public string? Notes { get; set; } // Optional notes about the operation
    }

    public enum InjectionStatus
    {
        Active,
        Completed,
        Stopped,
        Failed
    }
}

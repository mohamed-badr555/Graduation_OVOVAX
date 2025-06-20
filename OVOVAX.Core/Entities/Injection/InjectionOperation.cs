using System;
using OVOVAX.Core.Entities.Identity;

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
        
        // User relationship
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
        
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

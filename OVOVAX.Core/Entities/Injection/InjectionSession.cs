using System;
using System.Collections.Generic;

namespace OVOVAX.Core.Entities.Injection
{
    public class InjectionSession : BaseEntity
    {
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double RangeOfInfrared { get; set; } // in mm
        public double StepOfInjection { get; set; } // in mm
        public double VolumeOfLiquid { get; set; } // in ml
        public int NumberOfElements { get; set; }
        public InjectionStatus Status { get; set; }
        public List<InjectionRecord> InjectionRecords { get; set; } = new();
    }

    public enum InjectionStatus
    {
        Idle,
        Active,
        Completed,
        Failed,
        Stopped
    }
}

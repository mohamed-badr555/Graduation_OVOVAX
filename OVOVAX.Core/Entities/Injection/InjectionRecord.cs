using System;

namespace OVOVAX.Core.Entities.Injection
{
    public class InjectionRecord : BaseEntity
    {
        public int InjectionSessionId { get; set; }
        public InjectionSession InjectionSession { get; set; } = null!;
        public DateTime InjectionTime { get; set; }
        public int EggNumber { get; set; }
        public double VolumeInjected { get; set; } // in ml
        public InjectionRecordStatus Status { get; set; }
    }

    public enum InjectionRecordStatus
    {
        Success,
        Failed
    }
}

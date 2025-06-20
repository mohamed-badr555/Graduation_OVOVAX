using System;
using OVOVAX.Core.Entities.Identity;

namespace OVOVAX.Core.Entities.ManualControl
{
    public class MovementCommand : BaseEntity
    {
        public DateTime Timestamp { get; set; }
        public MovementAction Action { get; set; }
        public Axis Axis { get; set; }
        public MovementDirection Direction { get; set; }
        public int Speed { get; set; } // percentage
        public int Steps { get; set; } // number of steps to move
        public MovementStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        
        // User relationship
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
    }

    public enum MovementAction
    {
        Move,
        Home
    }

    public enum Axis
    {
        Z,
        Y,
        All
    }

    public enum MovementDirection
    {
        Positive = 1,
        Negative = -1
    }

    public enum MovementStatus
    {
        Completed,
        Failed,
        InProgress
    }
}

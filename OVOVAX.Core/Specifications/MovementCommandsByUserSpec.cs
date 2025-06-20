using OVOVAX.Core.Entities.ManualControl;

namespace OVOVAX.Core.Specifications
{
    public class MovementCommandsByUserSpec : BaseSpecification<MovementCommand>
    {
        public MovementCommandsByUserSpec(string userId) : base(x => x.UserId == userId)
        {
            AddOrderByDescending(x => x.CreatedAt);
        }
        
        public MovementCommandsByUserSpec(string userId, int commandId) : base(x => x.UserId == userId && x.ID == commandId)
        {
        }
    }
}

using OVOVAX.Core.Entities.ManualControl;
using OVOVAX.Core.Specifications;

namespace OVOVAX.Core.Specifications.ManualControl
{
    public class RecentMovementsSpecification : BaseSpecification<MovementCommand>
    {
        public RecentMovementsSpecification(int take = 10) : base()
        {
            AddOrderByDescending(x => x.Timestamp);
            ApplyPaging(0, take);
        }
    }
}

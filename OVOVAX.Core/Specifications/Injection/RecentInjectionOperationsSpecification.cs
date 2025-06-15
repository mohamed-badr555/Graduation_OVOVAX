using OVOVAX.Core.Entities.Injection;
using OVOVAX.Core.Specifications;

namespace OVOVAX.Core.Specifications.Injection
{
    public class RecentInjectionOperationsSpecification : BaseSpecification<InjectionOperation>
    {
        public RecentInjectionOperationsSpecification(int take = 10) : base()
        {
            AddOrderByDescending(x => x.StartTime);
            ApplyPaging(0, take);
        }
    }
}

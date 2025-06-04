using OVOVAX.Core.Entities.Injection;
using OVOVAX.Core.Specifications;

namespace OVOVAX.Core.Specifications.Injection
{
    public class RecentInjectionRecordsSpecification : BaseSpecification<InjectionRecord>
    {
        public RecentInjectionRecordsSpecification(int take = 10) : base()
        {
            AddOrderByDescending(x => x.InjectionTime);
            ApplyPaging(0, take);
        }
    }
}

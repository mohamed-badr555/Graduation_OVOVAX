using OVOVAX.Core.Entities.Injection;
using OVOVAX.Core.Specifications;

namespace OVOVAX.Core.Specifications.Injection
{
    public class InjectionSessionWithRecordsSpecification : BaseSpecification<InjectionSession>
    {
        public InjectionSessionWithRecordsSpecification(int sessionId) : base(x => x.ID == sessionId)
        {
            AddInclude(x => x.InjectionRecords);
        }
    }
}

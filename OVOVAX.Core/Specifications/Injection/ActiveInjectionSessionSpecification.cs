using OVOVAX.Core.Entities.Injection;
using OVOVAX.Core.Specifications;

namespace OVOVAX.Core.Specifications.Injection
{
    public class ActiveInjectionSessionSpecification : BaseSpecification<InjectionSession>
    {
        public ActiveInjectionSessionSpecification() : base(x => x.Status == InjectionStatus.Active)
        {
        }
    }
}

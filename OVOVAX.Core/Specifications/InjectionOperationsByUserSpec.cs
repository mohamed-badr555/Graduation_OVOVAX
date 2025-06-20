using OVOVAX.Core.Entities.Injection;

namespace OVOVAX.Core.Specifications
{
    public class InjectionOperationsByUserSpec : BaseSpecification<InjectionOperation>
    {
        public InjectionOperationsByUserSpec(string userId) : base(x => x.UserId == userId)
        {
            AddOrderByDescending(x => x.CreatedAt);
        }
        
        public InjectionOperationsByUserSpec(string userId, int operationId) : base(x => x.UserId == userId && x.ID == operationId)
        {
        }
    }
}

using OVOVAX.Core.Entities.Scanner;

namespace OVOVAX.Core.Specifications
{
    public class ScanResultsByUserSpec : BaseSpecification<ScanResult>
    {
        public ScanResultsByUserSpec(string userId) : base(x => x.UserId == userId)
        {
            AddOrderByDescending(x => x.CreatedAt);
        }
        
        public ScanResultsByUserSpec(string userId, int scanId) : base(x => x.UserId == userId && x.ID == scanId)
        {
        }
    }
}

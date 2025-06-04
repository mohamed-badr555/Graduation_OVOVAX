using OVOVAX.Core.Entities.Scanner;
using OVOVAX.Core.Specifications;

namespace OVOVAX.Core.Specifications.Scanner
{
    public class RecentScansSpecification : BaseSpecification<ScanResult>
    {
        public RecentScansSpecification(int take = 10) : base()
        {
            AddOrderByDescending(x => x.ScanTime);
            ApplyPaging(0, take);
        }
    }
}

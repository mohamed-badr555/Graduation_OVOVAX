using System.Collections.Generic;
using System.Threading.Tasks;
using OVOVAX.Core.Entities.Scanner;

namespace OVOVAX.Core.Interfaces
{
    public interface IScannerService
    {
        Task<ScanResult> StartScanAsync();
        Task<ScanResult> StopScanAsync(int scanId);
        Task<IEnumerable<ScanResult>> GetScanHistoryAsync();
    }
}

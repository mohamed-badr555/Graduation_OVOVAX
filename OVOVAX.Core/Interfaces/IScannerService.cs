using System.Collections.Generic;
using System.Threading.Tasks;
using OVOVAX.Core.Entities.Scanner;

namespace OVOVAX.Core.Interfaces
{
    public interface IScannerService
    {
        Task<ScanResult> StartScanAsync(string userId);
        Task<ScanResult> StopScanAsync(string userId, int scanId);
        Task<IEnumerable<ScanResult>> GetScanHistoryAsync(string userId);
    }
}

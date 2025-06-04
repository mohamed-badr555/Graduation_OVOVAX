using System.Collections.Generic;
using System.Threading.Tasks;
using OVOVAX.Core.DTOs.Scanner;

namespace OVOVAX.Core.Interfaces
{
    public interface IScannerService
    {
        Task<ScanResponseDto> StartScanAsync(ScanRequestDto request);
        Task<ScanResponseDto> StopScanAsync();
        Task<IEnumerable<ScanResultDto>> GetScanHistoryAsync();
        Task<object> GetScannerStatusAsync();
    }
}

using OVOVAX.Core.Models;

namespace OVOVAX.Core.Interfaces
{
    public interface IPythonApiService
    {
        Task<TrackDetectionResult> DetectTrackAsync();
        Task<CenterDetectionResult> DetectCenterAsync();
        Task<bool> CheckHealthAsync();
    }
}

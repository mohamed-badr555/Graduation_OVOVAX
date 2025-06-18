using OVOVAX.Core.Models;

namespace OVOVAX.Core.Interfaces
{
    public interface IPythonApiService
    {
        Task<TrackDetectionResult> DetectTrackAsync();
        Task<TrackDetectionResult> DetectTrackWithRetryAsync(int maxRetries = 3, int delaySeconds = 10);
        Task<CenterDetectionResult> DetectCenterAsync();
        Task<CenterDetectionResult> DetectCenterWithRetryAsync(int maxRetries = 3, int delaySeconds = 10);
        Task<bool> CheckHealthAsync();
    }
}

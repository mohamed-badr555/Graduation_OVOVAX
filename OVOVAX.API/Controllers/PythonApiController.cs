using Microsoft.AspNetCore.Mvc;
using OVOVAX.Core.Interfaces;
using OVOVAX.Core.Models;

namespace OVOVAX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PythonApiController : ControllerBase
    {
        private readonly IPythonApiService _pythonApiService;
        private readonly ILogger<PythonApiController> _logger;

        public PythonApiController(
            IPythonApiService pythonApiService,
            ILogger<PythonApiController> logger)
        {
            _pythonApiService = pythonApiService;
            _logger = logger;
        }        /// <summary>
        /// Detects track using the Python API on Raspberry Pi
        /// </summary>
        /// <returns>Track detection result</returns>
        [HttpGet("detect-track")]
        public async Task<ActionResult<TrackDetectionResult>> DetectTrack()
        {
            try
            {
                _logger.LogInformation("Track detection requested");
                
                // Use retry method for better reliability with long-running AI operations
                var result = await _pythonApiService.DetectTrackWithRetryAsync(maxRetries: 2, delaySeconds: 10);
                
                if (result.Success)
                {
                    _logger.LogInformation($"Track detection successful: {result.TrackId}");
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning($"Track detection failed: {result.ErrorMessage}");
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during track detection");
                return StatusCode(500, new TrackDetectionResult
                {
                    Success = false,
                    ErrorMessage = "Internal server error during track detection"
                });
            }
        }        /// <summary>
        /// Detects centers of objects using YOLO model on Raspberry Pi
        /// </summary>
        /// <returns>Center detection result with object count and coordinates</returns>
        [HttpGet("detect-center")]
        public async Task<ActionResult<CenterDetectionResult>> DetectCenter()
        {
            try
            {
                _logger.LogInformation("Center detection requested");
                
                // Use retry method for better reliability with long-running AI operations
                var result = await _pythonApiService.DetectCenterWithRetryAsync(maxRetries: 2, delaySeconds: 10);
                
                if (result.Success)
                {
                    _logger.LogInformation($"Center detection successful: {result.Count} objects detected");
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning($"Center detection failed: {result.ErrorMessage}");
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during center detection");
                return StatusCode(500, new CenterDetectionResult
                {
                    Success = false,
                    ErrorMessage = "Internal server error during center detection",
                    Timestamp = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Checks the health status of the Python API
        /// </summary>
        /// <returns>Health status</returns>
        [HttpGet("health")]
        public async Task<ActionResult> CheckHealth()
        {
            try
            {
                _logger.LogInformation("Python API health check requested");
                
                var isHealthy = await _pythonApiService.CheckHealthAsync();
                
                if (isHealthy)
                {
                    _logger.LogInformation("Python API is healthy");
                    return Ok(new { status = "healthy", message = "Python API is responding" });
                }
                else
                {
                    _logger.LogWarning("Python API health check failed");
                    return ServiceUnavailable(new { status = "unhealthy", message = "Python API is not responding" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Python API health check");
                return StatusCode(500, new { status = "error", message = "Error checking Python API health" });
            }
        }

        /// <summary>
        /// Gets the current Python API configuration
        /// </summary>
        /// <returns>API configuration information</returns>
        [HttpGet("config")]
        public ActionResult GetConfiguration()
        {
            try
            {
                _logger.LogInformation("Python API configuration requested");
                
                // This could be expanded to return actual configuration details
                return Ok(new 
                { 
                    message = "Python API configuration endpoint",
                    timestamp = DateTime.UtcNow,
                    status = "Configuration available via health endpoint"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Python API configuration");
                return StatusCode(500, new { message = "Error getting configuration" });
            }
        }

        /// <summary>
        /// Returns service unavailable with proper status code
        /// </summary>
        private ActionResult ServiceUnavailable(object value)
        {
            return StatusCode(503, value);
        }
    }
}

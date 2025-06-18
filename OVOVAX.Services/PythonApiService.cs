using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OVOVAX.Core.Interfaces;
using OVOVAX.Core.Models;

namespace OVOVAX.Services
{
    public class PythonApiService : IPythonApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _pythonApiBaseUrl;
        private readonly ILogger<PythonApiService> _logger;        public PythonApiService(HttpClient httpClient, IConfiguration configuration, ILogger<PythonApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            // Get hostname URL from configuration
            _pythonApiBaseUrl = configuration["PythonApi:BaseUrl"] ?? "http://raspberrypi.local:5000";
            
            // Increase timeout for AI model processing (5 minutes)
            var timeoutSeconds = configuration.GetValue<int>("PythonApi:TimeoutSeconds", 300);
            _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            
            _logger.LogInformation($"Python API configured for: {_pythonApiBaseUrl} with timeout: {timeoutSeconds}s");
        }

        public async Task<TrackDetectionResult> DetectTrackAsync()
        {
            try
            {
                _logger.LogInformation($"Sending request to Python API: {_pythonApiBaseUrl}/api/track/detect");
                
                var response = await _httpClient.GetAsync($"{_pythonApiBaseUrl}/api/track/detect");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Python API returned {response.StatusCode}: {errorContent}");
                    
                    return new TrackDetectionResult
                    {
                        Success = false,
                        ErrorMessage = $"Python API returned {response.StatusCode}: {errorContent}"
                    };
                }
                
                var responseJson = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Python API response: {responseJson}");
                
                var result = JsonSerializer.Deserialize<PythonApiResponse>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return new TrackDetectionResult
                {
                    Success = result?.Success ?? false,
                    TrackId = result?.TrackId ?? string.Empty,
                    DetectedTexts = result?.DetectedTexts ?? new List<DetectedText>(),
                    ErrorMessage = result?.Error ?? string.Empty
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Network error connecting to Python API: {ex.Message}");
                return new TrackDetectionResult
                {
                    Success = false,
                    ErrorMessage = $"Cannot connect to Raspberry Pi at {_pythonApiBaseUrl}. Check network connection."
                };
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError($"Timeout connecting to Python API: {ex.Message}");
                return new TrackDetectionResult
                {
                    Success = false,
                    ErrorMessage = "Timeout connecting to Raspberry Pi. Check if the device is running."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error communicating with Python API: {ex.Message}");
                return new TrackDetectionResult
                {
                    Success = false,
                    ErrorMessage = $"Error communicating with Python API: {ex.Message}"
                };
            }
        }

        public async Task<CenterDetectionResult> DetectCenterAsync()
        {
            try
            {
                _logger.LogInformation($"Sending center detection request to Python API: {_pythonApiBaseUrl}/api/center/detect");
                
                var response = await _httpClient.GetAsync($"{_pythonApiBaseUrl}/api/center/detect");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Python API returned {response.StatusCode}: {errorContent}");
                    
                    return new CenterDetectionResult
                    {
                        Success = false,
                        ErrorMessage = $"Python API returned {response.StatusCode}: {errorContent}",
                        Timestamp = DateTime.UtcNow
                    };
                }
                
                var responseJson = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Python API center detection response: {responseJson}");
                
                var result = JsonSerializer.Deserialize<CenterDetectionApiResponse>(responseJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return new CenterDetectionResult
                {
                    Success = result?.Success ?? false,
                    Count = result?.Count ?? 0,
                    Centers = result?.Centers ?? new List<DetectedCenter>(),
                    ErrorMessage = result?.Error ?? string.Empty,
                    Timestamp = result?.Timestamp > 0 ? DateTimeOffset.FromUnixTimeSeconds((long)result.Timestamp).DateTime : DateTime.UtcNow
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"Network error connecting to Python API for center detection: {ex.Message}");
                return new CenterDetectionResult
                {
                    Success = false,
                    ErrorMessage = $"Cannot connect to Raspberry Pi at {_pythonApiBaseUrl}. Check network connection.",
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError($"Timeout connecting to Python API for center detection: {ex.Message}");
                return new CenterDetectionResult
                {
                    Success = false,
                    ErrorMessage = "Timeout connecting to Raspberry Pi. Check if the device is running.",
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error communicating with Python API for center detection: {ex.Message}");
                return new CenterDetectionResult
                {
                    Success = false,
                    ErrorMessage = $"Error communicating with Python API: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };
            }
        }        public async Task<TrackDetectionResult> DetectTrackWithRetryAsync(int maxRetries = 2, int delaySeconds = 5)
        {
            TrackDetectionResult? lastResult = null;
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    _logger.LogInformation($"Track detection attempt {attempt}/{maxRetries}");
                    
                    lastResult = await DetectTrackAsync();
                    
                    if (lastResult.Success)
                    {
                        _logger.LogInformation($"Track detection successful on attempt {attempt}");
                        return lastResult;
                    }
                    
                    if (attempt < maxRetries)
                    {
                        _logger.LogWarning($"Track detection failed on attempt {attempt}, retrying in {delaySeconds} seconds...");
                        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception on track detection attempt {attempt}: {ex.Message}");
                    if (attempt == maxRetries)
                    {
                        return new TrackDetectionResult
                        {
                            Success = false,
                            ErrorMessage = $"Failed after {maxRetries} attempts. Last error: {ex.Message}"
                        };
                    }
                    
                    if (attempt < maxRetries)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                    }
                }
            }

            return lastResult ?? new TrackDetectionResult
            {
                Success = false,
                ErrorMessage = $"Failed after {maxRetries} attempts"
            };
        }        public async Task<CenterDetectionResult> DetectCenterWithRetryAsync(int maxRetries = 2, int delaySeconds = 5)
        {
            CenterDetectionResult? lastResult = null;
            
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    _logger.LogInformation($"Center detection attempt {attempt}/{maxRetries}");
                    
                    lastResult = await DetectCenterAsync();
                    
                    if (lastResult.Success)
                    {
                        _logger.LogInformation($"Center detection successful on attempt {attempt}");
                        return lastResult;
                    }
                    
                    if (attempt < maxRetries)
                    {
                        _logger.LogWarning($"Center detection failed on attempt {attempt}, retrying in {delaySeconds} seconds...");
                        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception on center detection attempt {attempt}: {ex.Message}");
                    if (attempt == maxRetries)
                    {
                        return new CenterDetectionResult
                        {
                            Success = false,
                            ErrorMessage = $"Failed after {maxRetries} attempts. Last error: {ex.Message}",
                            Timestamp = DateTime.UtcNow
                        };
                    }
                    
                    if (attempt < maxRetries)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                    }
                }
            }

            return lastResult ?? new CenterDetectionResult
            {
                Success = false,
                ErrorMessage = $"Failed after {maxRetries} attempts",
                Timestamp = DateTime.UtcNow
            };
        }

        public async Task<bool> CheckHealthAsync()
        {
            try
            {
                _logger.LogInformation($"Checking Python API health: {_pythonApiBaseUrl}/api/health");
                
                var response = await _httpClient.GetAsync($"{_pythonApiBaseUrl}/api/health");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Python API health check returned {response.StatusCode}: {errorContent}");
                    return false;
                }
                  var responseJson = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Python API health response: {responseJson}");
                
                // Simply return true if we get a successful response
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking Python API health: {ex.Message}");
                return false;
            }
        }
    }
}
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
        private readonly ILogger<PythonApiService> _logger;

        public PythonApiService(HttpClient httpClient, IConfiguration configuration, ILogger<PythonApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            
            // Get hostname URL from configuration
            _pythonApiBaseUrl = configuration["PythonApi:BaseUrl"] ?? "http://raspberrypi.local:5000";
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            _logger.LogInformation($"Python API configured for: {_pythonApiBaseUrl}");
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
        }

        public async Task<bool> CheckHealthAsync()
        {
            try
            {
                _logger.LogInformation($"Checking Python API health: {_pythonApiBaseUrl}/api/health");
                
                var response = await _httpClient.GetAsync($"{_pythonApiBaseUrl}/api/health");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"Python API health check successful: {content}");
                    return true;
                }
                
                _logger.LogWarning($"Python API health check failed: {response.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Python API health check error: {ex.Message}");
                return false;
            }
        }
    }
}

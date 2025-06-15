using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using OVOVAX.Core.Interfaces;

namespace OVOVAX.Services
{
    public class Esp32Service : IEsp32Service
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<Esp32Service> _logger;
        private readonly string _esp32BaseUrl;

        public Esp32Service(HttpClient httpClient, ILogger<Esp32Service> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _esp32BaseUrl = configuration["ESP32:BaseUrl"] ?? "http://192.168.1.100";
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<string> SendCommandAsync(string endpoint, object? data = null)
        {
            try
            {
                var url = $"{_esp32BaseUrl}/{endpoint}";
                
                if (data == null)
                {
                    var response = await _httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"ESP32 GET {endpoint}: {result}");
                    return result;
                }                else
                {
                    var json = JsonSerializer.Serialize(data);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"ESP32 POST {endpoint}: {result}");
                    return result;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"ESP32 HTTP error for {endpoint}");
                throw new Exception($"ESP32 connection failed: {ex.Message}");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, $"ESP32 timeout for {endpoint}");
                throw new Exception($"ESP32 timeout: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ESP32 communication failed for {endpoint}");
                throw new Exception($"ESP32 error: {ex.Message}");
            }
        }

        public async Task<T> GetDataAsync<T>(string endpoint)
        {            try
            {
                var response = await SendCommandAsync(endpoint);
                return JsonSerializer.Deserialize<T>(response) ?? throw new Exception("Failed to deserialize response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get data from ESP32 {endpoint}");
                throw;
            }
        }

        public async Task<bool> IsConnectedAsync()
        {
            try
            {
                await SendCommandAsync("ping");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

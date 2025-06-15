using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OVOVAX.Core.Entities.Scanner;
using OVOVAX.Core.Interfaces;
using OVOVAX.Core.Specifications.Scanner;

namespace OVOVAX.Services
{
    public class ScannerService : IScannerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEsp32Service _esp32Service;
        private readonly ILogger<ScannerService> _logger;
        
        // Store readings for active scans
        private readonly ConcurrentDictionary<int, List<double>> _activeScanReadings;
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _scanCancellationTokens;
        private readonly ConcurrentDictionary<int, Task> _scanTasks;

        public ScannerService(IUnitOfWork unitOfWork, IEsp32Service esp32Service, ILogger<ScannerService> logger)
        {
            _unitOfWork = unitOfWork;
            _esp32Service = esp32Service;
            _logger = logger;
            _activeScanReadings = new ConcurrentDictionary<int, List<double>>();
            _scanCancellationTokens = new ConcurrentDictionary<int, CancellationTokenSource>();
            _scanTasks = new ConcurrentDictionary<int, Task>();
        }        public async Task<ScanResult> StartScanAsync()
        {
            try
            {
                _logger.LogInformation("Starting new scan operation");

                // Create scan record with InProgress status
                var scanResult = new ScanResult
                {
                    ScanTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")),
                    Status = ScanStatus.InProgress,
                    SensorReadings = new List<double>()
                };

                await _unitOfWork.Repository<ScanResult>().Add(scanResult);
                await _unitOfWork.Complete();                // Send start scan command to ESP32 (GET request - starts scanning process)
                var esp32Response = await _esp32Service.SendCommandAsync("scanner/start");
                var startResponse = JsonSerializer.Deserialize<JsonElement>(esp32Response);
                bool success = startResponse.GetProperty("success").GetBoolean();

                if (success)
                {
                    _logger.LogInformation($"Scan {scanResult.ID} started successfully on ESP32");
                    // Keep status as InProgress - readings will be collected when stop is called
                }
                else
                {
                    scanResult.Status = ScanStatus.Failed;
                    _unitOfWork.Repository<ScanResult>().Update(scanResult);
                    await _unitOfWork.Complete();
                    _logger.LogError($"Failed to start scan {scanResult.ID} on ESP32");
                }

                return scanResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start scan");
                throw;
            }
        }        public async Task<ScanResult> StopScanAsync(int scanId)
        {
            try
            {
                _logger.LogInformation($"Stopping scan {scanId}");

                // Find the scan record
                var scanResult = await _unitOfWork.Repository<ScanResult>().GetByIdAsync(scanId);
                if (scanResult == null)
                {
                    throw new ArgumentException($"Scan with ID {scanId} not found");
                }

                if (scanResult.Status != ScanStatus.InProgress)
                {
                    throw new InvalidOperationException($"Cannot stop scan {scanId}. Current status is {scanResult.Status}");
                }

                // Send stop scan command to ESP32 (POST request - returns readings)
                var esp32Response = await _esp32Service.SendCommandAsync("scanner/stop", new { });
                var stopResponse = JsonSerializer.Deserialize<JsonElement>(esp32Response);
                bool success = stopResponse.GetProperty("success").GetBoolean();

                if (success && stopResponse.TryGetProperty("readings", out var readingsElement))
                {
                    // Get readings from ESP32 response
                    var readings = new List<double>();
                    foreach (var reading in readingsElement.EnumerateArray())
                    {
                        readings.Add(reading.GetDouble());
                    }
                    
                    scanResult.SensorReadings = readings;
                    scanResult.Status = ScanStatus.Success;
                    _logger.LogInformation($"Scan {scanId} completed with {readings.Count} readings");
                }
                else
                {
                    scanResult.Status = ScanStatus.Failed;
                    _logger.LogError($"Failed to get readings from ESP32 for scan {scanId}");
                }

                _unitOfWork.Repository<ScanResult>().Update(scanResult);
                await _unitOfWork.Complete();

                return scanResult;
            }
            catch (Exception ex)
            {
                var scanResult = await _unitOfWork.Repository<ScanResult>().GetByIdAsync(scanId);
                if (scanResult != null)
                {
                    scanResult.Status = ScanStatus.Failed;
                    _unitOfWork.Repository<ScanResult>().Update(scanResult);
                    await _unitOfWork.Complete();
                }

                _logger.LogError(ex, $"Failed to stop scan {scanId}");
                throw;
            }
        }

        public async Task<IEnumerable<ScanResult>> GetScanHistoryAsync()
        {
            var spec = new RecentScansSpecification(10);
            var scanResults = await _unitOfWork.Repository<ScanResult>().ListAsync(spec);
            return scanResults;
        }

        private async Task CollectSensorReadings(int scanId, CancellationToken cancellationToken)
        {
            var random = new Random();
            var readingInterval = TimeSpan.FromMilliseconds(100); // Read every 100ms
            
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    // Simulate sensor reading (replace with actual ESP32 communication)
                    var reading = 10.0 + (random.NextDouble() * 10.0); // Random reading between 10-20
                    
                    if (_activeScanReadings.TryGetValue(scanId, out var readings))
                    {
                        readings.Add(reading);
                        _logger.LogDebug($"Scan {scanId}: Added reading {reading:F2}");
                    }
                    
                    await Task.Delay(readingInterval, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Scan {scanId} readings collection cancelled");
            }
            catch (Exception ex)
            {                _logger.LogError(ex, $"Error collecting readings for scan {scanId}");
            }
        }
    }
}

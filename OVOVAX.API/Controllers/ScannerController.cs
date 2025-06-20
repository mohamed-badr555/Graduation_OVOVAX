using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using OVOVAX.API.DTOs.Scanner;
using OVOVAX.Core.Interfaces;
using OVOVAX.Core.Models;
using System.Security.Claims;

namespace OVOVAX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require authentication for all endpoints
    public class ScannerController : ControllerBase
    {
        private readonly IScannerService _scannerService;
        private readonly IPythonApiService _pythonApiService;
        private readonly IMapper _mapper;

        public ScannerController(IScannerService scannerService, IPythonApiService pythonApiService, IMapper mapper)
        {
            _scannerService = scannerService;
            _pythonApiService = pythonApiService;
            _mapper = mapper;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
        [HttpGet("start")]
        public async Task<ActionResult<ScanResponseDto>> StartScan()        {
            try
            {
                var userId = GetCurrentUserId();
                var scanResult = await _scannerService.StartScanAsync(userId);    
                
                var response = new ScanResponseDto
                {
                    Success = true,
                    Message = "Scan started successfully",
                    ScanId = scanResult.ID,
                    Status = scanResult.Status.ToString(),
                    Readings = null,
                    ReadingCount = 0
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ScanResponseDto
                {
                    Success = false,
                    Message = $"Failed to start scan: {ex.Message}"
                };
                return BadRequest(response);
            }
        }        [HttpPost("stop")]
        public async Task<ActionResult<ScanResponseDto>> StopScan([FromBody] StopScanDto request)        {
            try
            {
                var userId = GetCurrentUserId();
                var scanResult = await _scannerService.StopScanAsync(userId, request.ScanId);
                var response = new ScanResponseDto
                {
                    Success = true,
                    Message = "Scan stopped successfully",
                    ScanId = scanResult.ID,
                    Status = scanResult.Status.ToString(),
                    Readings = scanResult.SensorReadings.ToArray(),
                    ReadingCount = scanResult.SensorReadings.Count
                };
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                var response = new ScanResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
                return NotFound(response);
            }
            catch (InvalidOperationException ex)
            {
                var response = new ScanResponseDto
                {
                    Success = false,
                    Message = ex.Message
                };
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = new ScanResponseDto
                {
                    Success = false,
                    Message = $"Failed to stop scan: {ex.Message}"
                };
                return BadRequest(response);
            }
        }
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<ScanResultDto>>> GetScanHistory()        {
            try
            {
                var userId = GetCurrentUserId();
                var scanResults = await _scannerService.GetScanHistoryAsync(userId);
                var resultDtos = _mapper.Map<IEnumerable<ScanResultDto>>(scanResults);
                return Ok(resultDtos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to get scan history: {ex.Message}");
            }
        }

        [HttpPost("detect-track")]
        public async Task<ActionResult<TrackDetectionResponseDto>> DetectTrack()
        {
            try
            {
                // Get track ID from Python API (Raspberry Pi camera + OCR)
                var detectionResult = await _pythonApiService.DetectTrackAsync();

                if (!detectionResult.Success)
                {
                    return BadRequest(new TrackDetectionResponseDto
                    {
                        Success = false,
                        Message = detectionResult.ErrorMessage ?? "Failed to detect track"
                    });
                }

                // Here you can add database check for injection history
                // var hasInjectionHistory = await CheckTrackInjectionHistoryAsync(detectionResult.TrackId);
                var hasInjectionHistory = false; // Placeholder for now

                return Ok(new TrackDetectionResponseDto
                {
                    Success = true,
                    Message = hasInjectionHistory 
                        ? $"Track {detectionResult.TrackId} already injected - scan blocked" 
                        : $"Track {detectionResult.TrackId} detected - scan allowed",
                    TrackId = detectionResult.TrackId,
                    HasPreviousInjection = hasInjectionHistory,
                    AllowScan = !hasInjectionHistory,
                    DetectedTexts = detectionResult.DetectedTexts?.Select(dt => new DetectedTextDto
                    {
                        Text = dt.Text,
                        Confidence = dt.Confidence
                    }).ToList() ?? new List<DetectedTextDto>()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new TrackDetectionResponseDto
                {
                    Success = false,
                    Message = $"Failed to detect track: {ex.Message}"
                });
            }
        }

        [HttpGet("test-python-connection")]
        public async Task<ActionResult> TestPythonApiConnection()
        {
            try
            {
                var isHealthy = await _pythonApiService.CheckHealthAsync();
                
                return Ok(new
                {
                    connected = isHealthy,
                    apiUrl = "http://raspberrypi.local:5001",
                    message = isHealthy ? "Python API is reachable" : "Cannot connect to Python API",
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    connected = false,
                    error = ex.Message,
                    message = "Failed to test Python API connection"
                });
            }
        }

  
    }
}

using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using OVOVAX.API.DTOs.Scanner;
using OVOVAX.Core.Interfaces;

namespace OVOVAX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]   
    public class ScannerController : ControllerBase
    {
        private readonly IScannerService _scannerService;
        private readonly IMapper _mapper;

        public ScannerController(IScannerService scannerService, IMapper mapper)
        {
            _scannerService = scannerService;
            _mapper = mapper;
        }    
        [HttpGet("start")]
        public async Task<ActionResult<ScanResponseDto>> StartScan()
        {
            try
            {
                var scanResult = await _scannerService.StartScanAsync();    
                
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
        public async Task<ActionResult<ScanResponseDto>> StopScan([FromBody] StopScanDto request)
        {
            try
            {
                var scanResult = await _scannerService.StopScanAsync(request.ScanId);
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
        public async Task<ActionResult<IEnumerable<ScanResultDto>>> GetScanHistory()
        {
            try
            {
                var scanResults = await _scannerService.GetScanHistoryAsync();
                var resultDtos = _mapper.Map<IEnumerable<ScanResultDto>>(scanResults);
                return Ok(resultDtos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to get scan history: {ex.Message}");
            }
        }

  
    }
}

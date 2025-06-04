using Microsoft.AspNetCore.Mvc;
using OVOVAX.Core.DTOs.Scanner;
using OVOVAX.Core.Interfaces;

namespace OVOVAX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScannerController : ControllerBase
    {
        private readonly IScannerService _scannerService;

        public ScannerController(IScannerService scannerService)
        {
            _scannerService = scannerService;
        }

        [HttpPost("start")]
        public async Task<ActionResult<ScanResponseDto>> StartScan([FromBody] ScanRequestDto request)
        {
            var result = await _scannerService.StartScanAsync(request);
            return Ok(result);
        }

        [HttpPost("stop")]
        public async Task<ActionResult<ScanResponseDto>> StopScan()
        {
            var result = await _scannerService.StopScanAsync();
            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<ScanResultDto>>> GetScanHistory()
        {
            var history = await _scannerService.GetScanHistoryAsync();
            return Ok(history);
        }

        [HttpGet("status")]
        public async Task<ActionResult<object>> GetScannerStatus()
        {
            var status = await _scannerService.GetScannerStatusAsync();
            return Ok(status);
        }
    }
}

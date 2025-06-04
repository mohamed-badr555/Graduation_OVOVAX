using Microsoft.AspNetCore.Mvc;
using OVOVAX.Core.DTOs.Injection;
using OVOVAX.Core.Interfaces;

namespace OVOVAX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InjectionController : ControllerBase
    {
        private readonly IInjectionService _injectionService;

        public InjectionController(IInjectionService injectionService)
        {
            _injectionService = injectionService;
        }

        [HttpPost("start")]
        public async Task<ActionResult<InjectionResponseDto>> StartInjection([FromBody] StartInjectionDto request)
        {
            var result = await _injectionService.StartInjectionAsync(request);
            return Ok(result);
        }

        [HttpPost("stop")]
        public async Task<ActionResult<InjectionResponseDto>> StopInjection()
        {
            var result = await _injectionService.StopInjectionAsync();
            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<InjectionHistoryDto>>> GetInjectionHistory()
        {
            var history = await _injectionService.GetInjectionHistoryAsync();
            return Ok(history);
        }

        [HttpGet("status")]
        public async Task<ActionResult<object>> GetInjectionStatus()
        {
            var status = await _injectionService.GetInjectionStatusAsync();
            return Ok(status);
        }
    }
}

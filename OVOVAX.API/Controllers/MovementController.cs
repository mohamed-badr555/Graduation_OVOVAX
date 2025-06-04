using Microsoft.AspNetCore.Mvc;
using OVOVAX.Core.DTOs.ManualControl;
using OVOVAX.Core.Interfaces;

namespace OVOVAX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovementController : ControllerBase
    {
        private readonly IMovementService _movementService;

        public MovementController(IMovementService movementService)
        {
            _movementService = movementService;
        }

        [HttpPost("move")]
        public async Task<ActionResult<MovementResponseDto>> MoveAxis([FromBody] MovementRequestDto request)
        {
            var result = await _movementService.MoveAxisAsync(request);
            return Ok(result);
        }

        [HttpPost("home")]
        public async Task<ActionResult<MovementResponseDto>> HomeAxes([FromBody] HomeRequestDto request)
        {
            var result = await _movementService.HomeAxesAsync(request);
            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<MovementHistoryDto>>> GetMovementHistory()
        {
            var history = await _movementService.GetMovementHistoryAsync();
            return Ok(history);
        }

        [HttpGet("status")]
        public async Task<ActionResult<object>> GetMovementStatus()
        {
            var status = await _movementService.GetMovementStatusAsync();
            return Ok(status);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using OVOVAX.API.DTOs.ManualControl;
using OVOVAX.Core.Interfaces;

namespace OVOVAX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovementController : ControllerBase
    {
        private readonly IMovementService _movementService;
        private readonly IMapper _mapper;

        public MovementController(IMovementService movementService, IMapper mapper)
        {
            _movementService = movementService;
            _mapper = mapper;
        }      
        [HttpPost("move")]
        //[ProducesResponseType(typeof(Move))]
        public async Task<ActionResult<MovementResponseDto>> MoveAxis([FromBody] MovementRequestDto request)
        {            try
            {
                var movementCommand = await _movementService.MoveAxisAsync(
                    request.Axis.ToString(), 
                    (int)request.Direction, 
                    request.Speed);
                
                var response = new MovementResponseDto
                {
                    Success = true,
                    Message = "Movement executed successfully",
                    MovementId = movementCommand.ID
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new MovementResponseDto
                {
                    Success = false,
                    Message = $"Failed to execute movement: {ex.Message}"
                };
                return BadRequest(response);
            }
        }  
        [HttpPost("home")]
        public async Task<ActionResult<MovementResponseDto>> HomeAxes([FromBody] HomeRequestDto request)
        {
            try
            {
                var movementCommand = await _movementService.HomeAxesAsync(request.Speed);
                
                var response = new MovementResponseDto
                {
                    Success = true,
                    Message = "All axes homed successfully",
                    MovementId = movementCommand.ID
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new MovementResponseDto
                {
                    Success = false,
                    Message = $"Failed to home axes: {ex.Message}"
                };
                return BadRequest(response);
            }
        }      
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<MovementHistoryDto>>> GetMovementHistory()
        {
            var movements = await _movementService.GetMovementHistoryAsync();
            var historyDtos = _mapper.Map<IEnumerable<MovementHistoryDto>>(movements);
            return Ok(historyDtos);
        }

        [HttpGet("status")]
        public async Task<ActionResult<object>> GetMovementStatus()
        {
            var status = await _movementService.GetMovementStatusAsync();
            return Ok(status);
        }
    }
}

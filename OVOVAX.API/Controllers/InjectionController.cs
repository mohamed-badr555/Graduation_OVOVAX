using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using OVOVAX.API.DTOs.Injection;
using OVOVAX.Core.Interfaces;
using OVOVAX.Core.Entities.Injection;

namespace OVOVAX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class InjectionController : ControllerBase
    {
        private readonly IInjectionService _injectionService;
        private readonly IMapper _mapper;

        public InjectionController(IInjectionService injectionService, IMapper mapper)
        {
            _injectionService = injectionService;
            _mapper = mapper;
        }
        [HttpPost("status")]
         [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<bool>> GetInjectionStatusCompletedOrNot([FromBody] StopInjectionDto request)
        {
 
                var operation = await _injectionService.FindIsCompleteOrNot(request.OperationId);
                if (operation == null)
                {
                    return NotFound(new InjectionResponseDto
                    {
                        Success = false,
                        Message = "Operation not found",
                        OperationId = request.OperationId
                    });
                }
                if (operation.Status != InjectionStatus.Completed)
                {
                    return Ok(false);
                }
                else
                {
                    return Ok(true);
                }

          
            
        }

        [HttpPost("start")]
        [ProducesResponseType(typeof(InjectionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InjectionResponseDto), StatusCodes.Status400BadRequest)]
         [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<InjectionResponseDto>> StartInjection([FromBody] StartInjectionDto request)
        {
            try
            {
                var injectionOperation = await _injectionService.StartInjectionAsync(
                    request.RangeOfInfraredFrom,
                    request.RangeOfInfraredTo,
                    request.StepOfInjection, 
                    request.VolumeOfLiquid, 
                    request.NumberOfElements);
                  var response = new InjectionResponseDto
                {
                    Success = true,
                    Message = "Injection started successfully",
                    OperationId = injectionOperation.ID
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new InjectionResponseDto
                {
                    Success = false,
                    Message = $"Failed to start injection: {ex.Message}"
                };
                return BadRequest(response);
            }
        }     
        [HttpPost("stop")]
        [ProducesResponseType(typeof(InjectionResponseDto), StatusCodes.Status200OK)]
       
        [ProducesResponseType(typeof(InjectionResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InjectionResponseDto>> StopInjection([FromBody] StopInjectionDto request)
        {
            try
            {
                var success = await _injectionService.StopInjectionAsync(request.OperationId);
                var response = new InjectionResponseDto
                {
                    Success = success,
                    Message = success ? "Injection stopped successfully" : "Failed to stop injection - operation not found or not active",
                    OperationId=request.OperationId,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new InjectionResponseDto
                {
                    Success = false,
                    Message = $"Failed to stop injection: {ex.Message}",
                    OperationId = request.OperationId,
                };
                return BadRequest(response);
            }
        }  
        [HttpGet("history")]
        [ProducesResponseType(typeof(IEnumerable<InjectionHistoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<InjectionHistoryDto>>> GetInjectionHistory()
        {
            try
            {
                var injectionOperations = await _injectionService.GetInjectionHistoryAsync();
                var historyDtos = _mapper.Map<IEnumerable<InjectionHistoryDto>>(injectionOperations);
                return Ok(historyDtos);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to get injection history: {ex.Message}");
            }
        }      
        [HttpPost("complete")]
        [ProducesResponseType(typeof(InjectionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(InjectionResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(InjectionResponseDto), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InjectionResponseDto>> CompleteInjection([FromBody] StopInjectionDto request) // Using StopInjectionDto as it has the same structure
        {
            try
            {
                var success = await _injectionService.CompleteInjectionAsync(request.OperationId);
                var response = new InjectionResponseDto
                {
                    Success = success,
                    Message = success ? "Injection completed successfully" : "Failed to complete injection - operation not found or not active",
                    OperationId = request.OperationId,
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new InjectionResponseDto
                {
                    Success = false,
                    Message = $"Failed to complete injection: {ex.Message}",
                    OperationId = request.OperationId,
                };
                return BadRequest(response);
            }
        }
    }
}

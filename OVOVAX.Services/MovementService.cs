using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using OVOVAX.Core.DTOs.ManualControl;
using OVOVAX.Core.Entities.ManualControl;
using OVOVAX.Core.Interfaces;
using OVOVAX.Core.Specifications;
using OVOVAX.Core.Specifications.ManualControl;

namespace OVOVAX.Services
{
    public class MovementService : IMovementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MovementService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MovementResponseDto> MoveAxisAsync(MovementRequestDto request)
        {
            try
            {
                // TODO: Hardware communication logic here
                // await hardwareService.MoveAxis(request.Axis, request.Direction, request.Step, request.Speed);

                var movementCommand = new MovementCommand
                {
                    Timestamp = DateTime.UtcNow,
                    Action = MovementAction.Move,
                    Axis = request.Axis.ToLower() == "z" ? Axis.Z : Axis.Y,
                    Direction = (MovementDirection)request.Direction,
                    Step = request.Step,
                    Speed = request.Speed,
                    Status = MovementStatus.Completed
                };

                _unitOfWork.Repository<MovementCommand>().Add(movementCommand);
                await _unitOfWork.Complete();

                return new MovementResponseDto
                {
                    Success = true,
                    Message = $"Moved {request.Axis} axis {(request.Direction > 0 ? "positive" : "negative")} by {request.Step}mm",
                    MovementId = movementCommand.ID
                };
            }
            catch (Exception ex)
            {
                return new MovementResponseDto
                {
                    Success = false,
                    Message = $"Failed to move axis: {ex.Message}"
                };
            }
        }

        public async Task<MovementResponseDto> HomeAxesAsync(HomeRequestDto request)
        {
            try
            {
                // TODO: Hardware communication logic here
                // await hardwareService.HomeAxes();

                var movementCommand = new MovementCommand
                {
                    Timestamp = DateTime.UtcNow,
                    Action = MovementAction.Home,
                    Axis = Axis.All,
                    Direction = MovementDirection.Positive,
                    Step = 0,
                    Speed = 50,
                    Status = MovementStatus.Completed
                };

                _unitOfWork.Repository<MovementCommand>().Add(movementCommand);
                await _unitOfWork.Complete();

                return new MovementResponseDto
                {
                    Success = true,
                    Message = "All axes homed successfully",
                    MovementId = movementCommand.ID
                };
            }
            catch (Exception ex)
            {
                return new MovementResponseDto
                {
                    Success = false,
                    Message = $"Failed to home axes: {ex.Message}"
                };
            }
        }

        public async Task<IEnumerable<MovementHistoryDto>> GetMovementHistoryAsync()
        {
            var spec = new RecentMovementsSpecification(10);
            var movements = await _unitOfWork.Repository<MovementCommand>().ListAsync(spec);
            return _mapper.Map<IEnumerable<MovementHistoryDto>>(movements);
        }        public async Task<object> GetMovementStatusAsync()
        {
            // TODO: Get actual movement status from hardware
            await Task.Delay(50); // Simulate hardware status check delay
            return new
            {
                IsConnected = true,
                ZAxisPosition = 0.0,
                YAxisPosition = 0.0,
                IsHomed = true,
                Status = "Ready"
            };
        }
    }
}

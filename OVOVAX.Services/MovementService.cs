using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using OVOVAX.Core.Entities.ManualControl;
using OVOVAX.Core.Interfaces;
using OVOVAX.Core.Specifications.ManualControl;

namespace OVOVAX.Services
{
    public class MovementService : IMovementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEsp32Service _esp32Service;

        public MovementService(IUnitOfWork unitOfWork, IEsp32Service esp32Service)
        {
            _unitOfWork = unitOfWork;
            _esp32Service = esp32Service;
        }  
        public async Task<MovementCommand> MoveAxisAsync(string axis, int direction, int speed = 50, int steps = 1000)
        {
            try
            {
                // Send command to ESP32
                var esp32Command = new
                {
                    action = "move",
                    axis = axis.ToUpper(),
                    direction = direction,
                    speed = speed,
                    steps = steps
                };

                var esp32Response = await _esp32Service.SendCommandAsync("movement/move", esp32Command);
                
                // Parse and validate axis parameter
                if (!Enum.TryParse<Axis>(axis, true, out var axisEnum))
                {
                    throw new ArgumentException($"Invalid axis: {axis}");
                }

                // Parse direction to enum
                var directionEnum = direction > 0 ? MovementDirection.Positive : MovementDirection.Negative;

                var movementCommand = new MovementCommand
                {
                    Timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")),
                    Action = MovementAction.Move,
                    Axis = axisEnum,
                    Direction = directionEnum,
                    Speed = speed,
                    Steps = steps,
                    Status = MovementStatus.Completed
                };

                await _unitOfWork.Repository<MovementCommand>().Add(movementCommand);
                await _unitOfWork.Complete();

                return movementCommand;
            }
            catch (Exception)
            {                var failedCommand = new MovementCommand
                {
                    Timestamp = DateTime.Now,
                    Action = MovementAction.Move,
                    Axis = Enum.TryParse<Axis>(axis, true, out var axisEnum) ? axisEnum : Axis.Z,
                    Direction = direction > 0 ? MovementDirection.Positive : MovementDirection.Negative,
                    Speed = speed,
                    Steps = steps,
                    Status = MovementStatus.Failed
                };

                await _unitOfWork.Repository<MovementCommand>().Add(failedCommand);
                await _unitOfWork.Complete();                throw;
            }
        }
        public async Task<MovementCommand> HomeAxesAsync(int speed = 50)
        {
            try
            {
                // Send home command to ESP32 - ESP32 will respond immediately and then do homing
                var esp32Command = new
                {
                    speed = speed
                };

                var esp32Response = await _esp32Service.SendCommandAsync("movement/home", esp32Command);
                var response = JsonSerializer.Deserialize<JsonElement>(esp32Response);
                bool success = response.GetProperty("success").GetBoolean();                // Create home command record in database with "In Progress" status
                // since homing takes time and ESP32 responds before completion
                var movementCommand = new MovementCommand
                {
                    Timestamp = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")),
                    Action = MovementAction.Home,
                    Axis = Axis.All,
                    Direction = MovementDirection.Positive,
                    Speed = speed,
                    Steps = 0, // Homing doesn't use specific steps
                    Status = success ? MovementStatus.Completed : MovementStatus.Failed
                };

                await _unitOfWork.Repository<MovementCommand>().Add(movementCommand);
                await _unitOfWork.Complete();

                return movementCommand;
            }
            catch (Exception)
            {                var failedCommand = new MovementCommand
                {
                    Timestamp = DateTime.Now,
                    Action = MovementAction.Home,
                    Axis = Axis.All,
                    Direction = MovementDirection.Positive,
                    Speed = speed,
                    Steps = 0, // Homing doesn't use specific steps
                    Status = MovementStatus.Failed
                };

                await _unitOfWork.Repository<MovementCommand>().Add(failedCommand);
                await _unitOfWork.Complete();

                throw;
            }
        }

        public async Task<object> GetMovementStatusAsync(int? homingOperationId = null)
        {
            try
            {
                // Get status from ESP32 to check if homing is complete
                var esp32Response = await _esp32Service.SendCommandAsync("movement/status");
                var response = JsonSerializer.Deserialize<JsonElement>(esp32Response);
                
                bool isHomed = response.GetProperty("isHomed").GetBoolean();
                bool limitSwitch1 = response.GetProperty("limitSwitch1").GetBoolean();
                bool limitSwitch2 = response.GetProperty("limitSwitch2").GetBoolean();                  // Check for specific homing operation and update its status if homing is complete
                if (isHomed && homingOperationId.HasValue)
                {
                    // Find the specific homing operation and mark it as completed
                    var homingOperation = await _unitOfWork.Repository<MovementCommand>()
                        .GetByIdAsync(homingOperationId.Value);
                    
                    if (homingOperation != null && 
                        homingOperation.Action == MovementAction.Home && 
                        homingOperation.Status == MovementStatus.InProgress)
                    {
                        homingOperation.Status = MovementStatus.Completed;
                        _unitOfWork.Repository<MovementCommand>().Update(homingOperation);
                        await _unitOfWork.Complete();
                    }
                }
                
                return new
                {
                    IsConnected = true,
                    ZAxisPosition = response.GetProperty("zPosition").GetDouble(),
                    YAxisPosition = response.GetProperty("yPosition").GetDouble(),
                    IsHomed = isHomed,
                    IsMoving = response.GetProperty("isMoving").GetBoolean(),
                    Status = isHomed ? "Ready" : "Homing in progress",
                    LimitSwitch1 = limitSwitch1,
                    LimitSwitch2 = limitSwitch2,
                    LimitSwitch3 = response.GetProperty("limitSwitch3").GetBoolean(),
                    Timestamp = response.GetProperty("timestamp").GetInt64()
                };
            }
            catch (Exception)
            {
                // If ESP32 communication fails, return basic status
                return new
                {
                    IsConnected = false,
                    ZAxisPosition = 0.0,
                    YAxisPosition = 0.0,
                    IsHomed = false,
                    IsMoving = false,
                    Status = "ESP32 Connection Failed",
                    LimitSwitch1 = false,
                    LimitSwitch2 = false,
                    LimitSwitch3 = false,
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
            }
        }

        public async Task<IEnumerable<MovementCommand>> GetMovementHistoryAsync()
        {
            var spec = new RecentMovementsSpecification(10);
            var movements = await _unitOfWork.Repository<MovementCommand>().ListAsync(spec);
            return movements;
        }      




    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using OVOVAX.Core.Entities.Injection;
using OVOVAX.Core.Interfaces;
using OVOVAX.Core.Specifications.Injection;

namespace OVOVAX.Services
{
    public class InjectionService : IInjectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEsp32Service _esp32Service;

        public InjectionService(IUnitOfWork unitOfWork, IEsp32Service esp32Service)
        {
            _unitOfWork = unitOfWork;
            _esp32Service = esp32Service;
        }   
        public async Task<InjectionOperation> StartInjectionAsync(double rangeOfInfraredfrom, double rangeOfInfraredto, double stepOfInjection, double volumeOfLiquid, int numberOfElements)
        {
            try
            {
                // Send injection command to ESP32
                var esp32Command = new
                {
                    rangeFrom = rangeOfInfraredfrom,
                    rangeTo = rangeOfInfraredto,
                    stepOfInjection = stepOfInjection,
                    volumeOfLiquid = volumeOfLiquid,
                    numberOfElements = numberOfElements
                };

                var esp32Response = await _esp32Service.SendCommandAsync("injection/start", esp32Command);
                var response = JsonSerializer.Deserialize<JsonElement>(esp32Response);
                bool success = response.GetProperty("success").GetBoolean();                // Create injection operation record in database
                var injectionOperation = new InjectionOperation
                {
                    StartTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")),
                    RangeOfInfraredFrom = rangeOfInfraredfrom,
                    RangeOfInfraredTo = rangeOfInfraredto,
                    StepOfInjection = stepOfInjection,
                    VolumeOfLiquid = volumeOfLiquid,
                    NumberOfElements = numberOfElements,       
                    Status = success ? InjectionStatus.Active : InjectionStatus.Failed,
                    EndTime = success ? null : DateTime.Now
                };

                await _unitOfWork.Repository<InjectionOperation>().Add(injectionOperation);
                await _unitOfWork.Complete();

                return injectionOperation;
            }
            catch (Exception)
            {                var failedOperation = new InjectionOperation
                {
                    StartTime = DateTime.Now,
                    RangeOfInfraredFrom = rangeOfInfraredfrom,
                    RangeOfInfraredTo = rangeOfInfraredto,
                    StepOfInjection = stepOfInjection,
                    VolumeOfLiquid = volumeOfLiquid,
                    NumberOfElements = numberOfElements,
                    Status = InjectionStatus.Failed
                };

                await _unitOfWork.Repository<InjectionOperation>().Add(failedOperation);
                await _unitOfWork.Complete();

                throw;
            }
        }      
        
        public async Task<bool> StopInjectionAsync(int operationId)
        {
            try
            {
                // Find and validate the specific operation first
                var operation = await _unitOfWork.Repository<InjectionOperation>()
                    .GetByIdAsync(operationId);

                if (operation == null)
                {
                    return false; // Operation not found
                }

                if (operation.Status != InjectionStatus.Active)
                {
                    return false; // Operation is not active, cannot stop
                }

                bool esp32Success = true;
                
                try
                {
                    // Try to send stop injection command to ESP32
                    var esp32Command = new { operationId = operationId };
                    var esp32Response = await _esp32Service.SendCommandAsync("injection/stop", esp32Command);
                    var response = JsonSerializer.Deserialize<JsonElement>(esp32Response);
                    esp32Success = response.GetProperty("success").GetBoolean();
                }
                catch (Exception)
                {
                    // If ESP32 communication fails, we'll still update the database
                    // This handles cases where ESP32 endpoint doesn't exist or is unreachable
                    esp32Success = false;
                }

                // Update the operation status regardless of ESP32 response
                operation.Status = InjectionStatus.Stopped;
                operation.EndTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"));
                _unitOfWork.Repository<InjectionOperation>().Update(operation);

                await _unitOfWork.Complete();
                
                // Return true if database update was successful, even if ESP32 communication failed
                return true;
            }
            catch (Exception)
            {
                throw; // Let the controller handle the exception
            }
        }  
              public async Task<IEnumerable<InjectionOperation>> GetInjectionHistoryAsync()
        {
            var injectionOperations = await _unitOfWork.Repository<InjectionOperation>()
                .ListAsync(new RecentInjectionOperationsSpecification(15));

            return injectionOperations;
        }
        public async Task<InjectionOperation?> FindIsCompleteOrNot(int operationId)
        {
            try
            {
                var operation = await _unitOfWork.Repository<InjectionOperation>()
                    .GetByIdAsync(operationId);

                if (operation == null)
                    return null;

                // If operation is still active, check ESP32 status
                if (operation.Status == InjectionStatus.Active)
                {
                    try
                    {
                        var esp32Command = new { operationId = operationId };
                        var esp32Response = await _esp32Service.SendCommandAsync("injection/status", esp32Command);
                        var response = JsonSerializer.Deserialize<JsonElement>(esp32Response);
                        
                        bool isRunning = response.GetProperty("isRunning").GetBoolean();
                        bool isCompleted = response.GetProperty("isCompleted").GetBoolean();
                        
                        // Update status based on ESP32 response
                        if (isCompleted)
                        {
                            operation.Status = InjectionStatus.Completed;
                            operation.EndTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"));
                            _unitOfWork.Repository<InjectionOperation>().Update(operation);
                            await _unitOfWork.Complete();
                        }
                        else if (!isRunning)
                        {
                            // Injection stopped but not completed
                            operation.Status = InjectionStatus.Stopped;
                            operation.EndTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                                TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"));
                            _unitOfWork.Repository<InjectionOperation>().Update(operation);
                            await _unitOfWork.Complete();
                        }
                    }
                    catch (Exception)
                    {
                        // If ESP32 communication fails, keep current status
                        // This prevents changing the status when we can't communicate with ESP32
                        // The operation remains "Active" until we can confirm its actual status
                    }
                }

                return operation;
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}

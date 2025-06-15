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
        }        public async Task<InjectionOperation> StartInjectionAsync(double rangeOfInfraredfrom, double rangeOfInfraredto, double stepOfInjection, double volumeOfLiquid, int numberOfElements)
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
                bool success = response.GetProperty("success").GetBoolean();

                // Create injection operation record in database
                var injectionOperation = new InjectionOperation
                {
                    StartTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")),
                    RangeOfInfraredFrom = rangeOfInfraredfrom,
                    RangeOfInfraredTo = rangeOfInfraredto,
                    StepOfInjection = stepOfInjection,
                    VolumeOfLiquid = volumeOfLiquid,
                    NumberOfElements = numberOfElements,                    Status = success ? InjectionStatus.Completed : InjectionStatus.Failed,
                    EndTime = success ? DateTime.Now : null
                };

                await _unitOfWork.Repository<InjectionOperation>().Add(injectionOperation);
                await _unitOfWork.Complete();

                return injectionOperation;
            }
            catch (Exception)
            {
                var failedOperation = new InjectionOperation
                {
                    StartTime = DateTime.Now,
                    RangeOfInfraredFrom = rangeOfInfraredfrom,
                    RangeOfInfraredTo = rangeOfInfraredto,
                    StepOfInjection = stepOfInjection,
                    VolumeOfLiquid = volumeOfLiquid,                    NumberOfElements = numberOfElements,
                    Status = InjectionStatus.Failed
                };

                await _unitOfWork.Repository<InjectionOperation>().Add(failedOperation);
                await _unitOfWork.Complete();

                throw;
            }
        }        public async Task<bool> StopInjectionAsync(int operationId)
        {
            try
            {
                // Send stop injection command to ESP32
                var esp32Response = await _esp32Service.SendCommandAsync("injection/stop");
                var response = JsonSerializer.Deserialize<JsonElement>(esp32Response);
                bool success = response.GetProperty("success").GetBoolean();

                // Find and update the specific operation
                var operation = await _unitOfWork.Repository<InjectionOperation>()
                    .GetByIdAsync(operationId);

                if (operation == null)
                {
                    return false; // Operation not found
                }

                operation.Status = success ? InjectionStatus.Stopped : InjectionStatus.Failed;
                operation.EndTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"));
                _unitOfWork.Repository<InjectionOperation>().Update(operation);

                await _unitOfWork.Complete();
                return success;
            }
            catch (Exception)
            {
                throw; // Let the controller handle the exception
            }
        }

        public async Task<bool> CompleteInjectionAsync(int operationId)
        {
            try
            {
                // Find and update the specific operation
                var operation = await _unitOfWork.Repository<InjectionOperation>()
                    .GetByIdAsync(operationId);

                if (operation == null || operation.Status != InjectionStatus.Active)
                {
                    return false; // Operation not found or not active
                }

                operation.Status = InjectionStatus.Completed;
                operation.EndTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                        TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")); ;
                _unitOfWork.Repository<InjectionOperation>().Update(operation);

                await _unitOfWork.Complete();
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
            var operation = await _unitOfWork.Repository<InjectionOperation>()
                .GetByIdAsync(operationId);
            return operation;
        }


    }
}

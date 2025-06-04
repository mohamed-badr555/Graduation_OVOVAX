using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OVOVAX.Core.DTOs.Injection;
using OVOVAX.Core.Entities.Injection;
using OVOVAX.Core.Interfaces;
using OVOVAX.Core.Specifications;
using OVOVAX.Core.Specifications.Injection;

namespace OVOVAX.Services
{
    public class InjectionService : IInjectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InjectionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<InjectionResponseDto> StartInjectionAsync(StartInjectionDto request)
        {
            try
            {
                // TODO: Hardware communication logic here
                // await hardwareService.StartInjection(request);

                var injectionSession = new InjectionSession
                {
                    StartTime = DateTime.UtcNow,
                    RangeOfInfrared = request.RangeOfInfrared,
                    StepOfInjection = request.StepOfInjection,
                    VolumeOfLiquid = request.VolumeOfLiquid,
                    NumberOfElements = request.NumberOfElements,
                    Status = InjectionStatus.Active
                };

                _unitOfWork.Repository<InjectionSession>().Add(injectionSession);
                await _unitOfWork.Complete();

                return new InjectionResponseDto
                {
                    Success = true,
                    Message = "Injection started successfully",
                    SessionId = injectionSession.ID
                };
            }
            catch (Exception ex)
            {
                return new InjectionResponseDto
                {
                    Success = false,
                    Message = $"Failed to start injection: {ex.Message}"
                };
            }
        }

        public async Task<InjectionResponseDto> StopInjectionAsync()
        {
            try
            {
                // TODO: Hardware communication logic here
                // await hardwareService.StopInjection();                // Find active session and update status
                var activeSessions = await _unitOfWork.Repository<InjectionSession>()
                    .ListAsync(new ActiveInjectionSessionSpecification());

                foreach (var session in activeSessions)
                {
                    session.Status = InjectionStatus.Stopped;
                    session.EndTime = DateTime.UtcNow;
                    _unitOfWork.Repository<InjectionSession>().Update(session);
                }

                await _unitOfWork.Complete();

                return new InjectionResponseDto
                {
                    Success = true,
                    Message = "Injection stopped successfully"
                };
            }
            catch (Exception ex)
            {
                return new InjectionResponseDto
                {
                    Success = false,
                    Message = $"Failed to stop injection: {ex.Message}"
                };
            }
        }        public async Task<IEnumerable<InjectionHistoryDto>> GetInjectionHistoryAsync()
        {
            // Get recent injection records
            var injectionRecords = await _unitOfWork.Repository<InjectionRecord>()
                .ListAsync(new RecentInjectionRecordsSpecification(10));

            return _mapper.Map<IEnumerable<InjectionHistoryDto>>(injectionRecords);
        }        public async Task<object> GetInjectionStatusAsync()
        {
            // TODO: Get actual injection status from hardware
            var activeSession = await _unitOfWork.Repository<InjectionSession>()
                .GetEntityWithSpec(new ActiveInjectionSessionSpecification());

            return new
            {
                IsActive = activeSession != null,
                CurrentSession = activeSession?.ID,
                Status = activeSession?.Status.ToString() ?? "Idle",
                Progress = activeSession != null ? 50 : 0 // TODO: Calculate actual progress
            };
        }
    }
}

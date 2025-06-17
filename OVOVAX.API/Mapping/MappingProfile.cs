using AutoMapper;
using OVOVAX.API.DTOs.Scanner;
using OVOVAX.API.DTOs.Injection;
using OVOVAX.API.DTOs.ManualControl;
using OVOVAX.Core.Entities.Scanner;
using OVOVAX.Core.Entities.Injection;
using OVOVAX.Core.Entities.ManualControl;

namespace OVOVAX.API.Mapping
{
    public class MappingProfile : Profile
    {        public MappingProfile()
        {
            // Scanner mappings
            // Scanner mappings
            CreateMap<ScanResult, ScanResultDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.ScanTime, opt => opt.MapFrom(src =>
                    TimeZoneInfo.ConvertTimeFromUtc(src.ScanTime,
                        TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"))))  
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));            // Injection mappings

            CreateMap<InjectionOperation, InjectionHistoryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString()))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime != null ? src.EndTime.ToString() : null))
                .ForMember(dest => dest.EggNumber, opt => opt.MapFrom(src => src.NumberOfElements))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.VolumeOfLiquid))
                .ForMember(dest => dest.RangeFrom, opt => opt.MapFrom(src => src.RangeOfInfraredFrom))
                .ForMember(dest => dest.RangeTo, opt => opt.MapFrom(src => src.RangeOfInfraredTo))
                .ForMember(dest => dest.Step, opt => opt.MapFrom(src => src.StepOfInjection))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
                 // Movement mappings
            CreateMap<MovementCommand, MovementHistoryDto>()
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp.ToString()))
                .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.Action.ToString()))
                .ForMember(dest => dest.Speed, opt => opt.MapFrom(src => src.Speed))
                .ForMember(dest => dest.Steps, opt => opt.MapFrom(src => src.Steps))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => src.Direction.ToString()));

        }
    }
}

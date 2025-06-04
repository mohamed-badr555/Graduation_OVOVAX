using AutoMapper;
using OVOVAX.Core.DTOs.Scanner;
using OVOVAX.Core.DTOs.Injection;
using OVOVAX.Core.DTOs.ManualControl;
using OVOVAX.Core.Entities.Scanner;
using OVOVAX.Core.Entities.Injection;
using OVOVAX.Core.Entities.ManualControl;

namespace OVOVAX.Services.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Scanner mappings
            CreateMap<ScanResult, ScanResultDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Injection mappings
            CreateMap<InjectionRecord, InjectionHistoryDto>()
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.InjectionTime.ToString("HH:mm:ss")))
                .ForMember(dest => dest.Volume, opt => opt.MapFrom(src => src.VolumeInjected))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Movement mappings
            CreateMap<MovementCommand, MovementHistoryDto>()
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp.ToString("HH:mm:ss")))
                .ForMember(dest => dest.Action, opt => opt.MapFrom(src => src.Action.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}

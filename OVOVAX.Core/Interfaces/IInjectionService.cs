using System.Collections.Generic;
using System.Threading.Tasks;
using OVOVAX.Core.DTOs.Injection;

namespace OVOVAX.Core.Interfaces
{
    public interface IInjectionService
    {
        Task<InjectionResponseDto> StartInjectionAsync(StartInjectionDto request);
        Task<InjectionResponseDto> StopInjectionAsync();
        Task<IEnumerable<InjectionHistoryDto>> GetInjectionHistoryAsync();
        Task<object> GetInjectionStatusAsync();
    }
}

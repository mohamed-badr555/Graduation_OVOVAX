using System.Collections.Generic;
using System.Threading.Tasks;
using OVOVAX.Core.Entities.Injection;

namespace OVOVAX.Core.Interfaces
{    public interface IInjectionService
    {
        Task<InjectionOperation> StartInjectionAsync(string userId, double rangeOfInfraredfrom, double rangeOfInfraredto, double stepOfInjection, double volumeOfLiquid, int numberOfElements);
        Task<bool> StopInjectionAsync(string userId, int operationId);
        Task<IEnumerable<InjectionOperation>> GetInjectionHistoryAsync(string userId);
        Task<InjectionOperation?> FindIsCompleteOrNot(string userId, int operationId);
    }
}

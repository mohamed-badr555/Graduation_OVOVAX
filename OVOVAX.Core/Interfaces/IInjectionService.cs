using System.Collections.Generic;
using System.Threading.Tasks;
using OVOVAX.Core.Entities.Injection;

namespace OVOVAX.Core.Interfaces
{    public interface IInjectionService
    {
        Task<InjectionOperation> StartInjectionAsync(double rangeOfInfraredfrom, double rangeOfInfraredto, double stepOfInjection, double volumeOfLiquid, int numberOfElements);
        Task<bool> StopInjectionAsync(int operationId);
        Task<bool> CompleteInjectionAsync(int operationId);
        Task<IEnumerable<InjectionOperation>> GetInjectionHistoryAsync();
        Task<InjectionOperation?> FindIsCompleteOrNot(int operationId);
    }
}

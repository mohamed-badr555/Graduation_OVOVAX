using System.Collections.Generic;
using System.Threading.Tasks;
using OVOVAX.Core.DTOs.ManualControl;

namespace OVOVAX.Core.Interfaces
{
    public interface IMovementService
    {
        Task<MovementResponseDto> MoveAxisAsync(MovementRequestDto request);
        Task<MovementResponseDto> HomeAxesAsync(HomeRequestDto request);
        Task<IEnumerable<MovementHistoryDto>> GetMovementHistoryAsync();
        Task<object> GetMovementStatusAsync();
    }
}

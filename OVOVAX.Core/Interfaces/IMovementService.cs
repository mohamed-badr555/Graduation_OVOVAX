using System.Collections.Generic;
using System.Threading.Tasks;
using OVOVAX.Core.Entities.ManualControl;

namespace OVOVAX.Core.Interfaces
{
    public interface IMovementService
    {
        Task<MovementCommand> MoveAxisAsync(string userId, string axis, int direction, int speed = 50, int steps = 1000);
        Task<MovementCommand> HomeAxesAsync(string userId, int speed = 50);
        Task<IEnumerable<MovementCommand>> GetMovementHistoryAsync(string userId);
        Task<object> GetMovementStatusAsync(string userId, int? homingOperationId = null);
    }
}

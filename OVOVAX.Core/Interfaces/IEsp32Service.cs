using System.Threading.Tasks;

namespace OVOVAX.Core.Interfaces
{
    public interface IEsp32Service
    {
        Task<string> SendCommandAsync(string endpoint, object data = null);
        Task<T> GetDataAsync<T>(string endpoint);
        Task<bool> IsConnectedAsync();
    }
}

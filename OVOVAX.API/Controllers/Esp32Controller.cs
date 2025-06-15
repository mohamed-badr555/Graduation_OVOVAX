using Microsoft.AspNetCore.Mvc;
using OVOVAX.Core.Interfaces;
using System.Threading.Tasks;

namespace OVOVAX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Esp32Controller : ControllerBase
    {
        private readonly IEsp32Service _esp32Service;

        public Esp32Controller(IEsp32Service esp32Service)
        {
            _esp32Service = esp32Service;
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetConnectionStatus()
        {
            try
            {
                var isConnected = await _esp32Service.IsConnectedAsync();
                return Ok(new { 
                    connected = isConnected, 
                    timestamp = DateTime.Now,
                    message = isConnected ? "ESP32 is connected" : "ESP32 is not connected"
                });
            }
            catch (Exception ex)
            {
                return Ok(new { 
                    connected = false, 
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var response = await _esp32Service.SendCommandAsync("ping");
                return Ok(new { 
                    success = true, 
                    response = response,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    success = false, 
                    error = ex.Message,
                    timestamp = DateTime.Now
                });
            }
        }
    }
}

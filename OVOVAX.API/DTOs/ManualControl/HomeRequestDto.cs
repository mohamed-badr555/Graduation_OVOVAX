using System.ComponentModel.DataAnnotations;

namespace OVOVAX.API.DTOs.ManualControl
{
    public class HomeRequestDto
    {
        [Required]
        [Range(1, 100, ErrorMessage = "Speed must be between 1 and 100")]
        public int Speed { get; set; } = 50; // Default speed 50%
    }
}

using System.ComponentModel.DataAnnotations;

namespace OVOVAX.API.DTOs.ManualControl
{
    public class MovementRequestDto
    {
        [Required]
        public string Axis { get; set; } = string.Empty; // "z" or "y"

        [Required]
        [Range(-1, 1, ErrorMessage = "Direction must be -1 or 1")]
        public int Direction { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Speed must be between 1 and 100")]
        public int Speed { get; set; }
    }
}

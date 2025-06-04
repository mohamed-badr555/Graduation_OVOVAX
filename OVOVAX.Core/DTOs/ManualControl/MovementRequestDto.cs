using System.ComponentModel.DataAnnotations;

namespace OVOVAX.Core.DTOs.ManualControl
{
    public class MovementRequestDto
    {
        [Required]
        public string Axis { get; set; } = string.Empty; // "z" or "y"

        [Required]
        [Range(-1, 1, ErrorMessage = "Direction must be -1 or 1")]
        public int Direction { get; set; }

        [Range(0.1, 100, ErrorMessage = "Step must be between 0.1 and 100 mm")]
        public double Step { get; set; } = 1.0;

        [Required]
        [Range(1, 100, ErrorMessage = "Speed must be between 1 and 100")]
        public int Speed { get; set; }
    }
}

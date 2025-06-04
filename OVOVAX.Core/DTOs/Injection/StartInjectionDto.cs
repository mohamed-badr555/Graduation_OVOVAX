using System.ComponentModel.DataAnnotations;

namespace OVOVAX.Core.DTOs.Injection
{
    public class StartInjectionDto
    {
        [Required]
        [Range(0.1, 1000, ErrorMessage = "Range must be between 0.1 and 1000 mm")]
        public double RangeOfInfrared { get; set; }

        [Required]
        [Range(0.1, 100, ErrorMessage = "Step must be between 0.1 and 100 mm")]
        public double StepOfInjection { get; set; }

        [Required]
        [Range(0.01, 10, ErrorMessage = "Volume must be between 0.01 and 10 ml")]
        public double VolumeOfLiquid { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Number of elements must be between 1 and 100")]
        public int NumberOfElements { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace OVOVAX.API.DTOs.Scanner
{
    public class StopScanDto
    {
        [Required]
        public int ScanId { get; set; }
    }
}

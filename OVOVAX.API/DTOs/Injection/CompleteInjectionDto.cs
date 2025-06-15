using System.ComponentModel.DataAnnotations;

namespace OVOVAX.API.DTOs.Injection
{
    public class CompleteInjectionDto
    {
        [Required]
        public int OperationId { get; set; }
    }
}

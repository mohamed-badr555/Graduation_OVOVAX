namespace OVOVAX.Core.DTOs.Scanner
{
    public class ScanResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? ScanId { get; set; }
    }
}

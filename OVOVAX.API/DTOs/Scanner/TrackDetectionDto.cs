namespace OVOVAX.API.DTOs.Scanner
{
    public class TrackDetectionResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TrackId { get; set; } = string.Empty;
        public bool HasPreviousInjection { get; set; }
        public bool AllowScan { get; set; }
        public List<DetectedTextDto> DetectedTexts { get; set; } = new();
    }

    public class DetectedTextDto
    {
        public string Text { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }
}

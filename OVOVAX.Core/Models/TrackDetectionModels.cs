namespace OVOVAX.Core.Models
{
    public class TrackDetectionResult
    {
        public bool Success { get; set; }
        public string TrackId { get; set; } = string.Empty;
        public List<DetectedText> DetectedTexts { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class DetectedText
    {
        public string Text { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }

    public class PythonApiResponse
    {
        public bool Success { get; set; }
        public string TrackId { get; set; } = string.Empty;
        public List<DetectedText> DetectedTexts { get; set; } = new();
        public string Error { get; set; } = string.Empty;
    }
}
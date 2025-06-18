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

    // New models for center detection
    public class CenterDetectionResult
    {
        public bool Success { get; set; }
        public int Count { get; set; }
        public List<DetectedCenter> Centers { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class DetectedCenter
    {
        public string Label { get; set; } = string.Empty;
        public double Confidence { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class CenterDetectionApiResponse
    {
        public bool Success { get; set; }
        public int Count { get; set; }
        public List<DetectedCenter> Centers { get; set; } = new();
        public string Error { get; set; } = string.Empty;
        public double Timestamp { get; set; }
    }
}
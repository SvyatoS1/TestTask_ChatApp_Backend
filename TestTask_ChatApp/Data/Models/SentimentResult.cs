using System.Globalization;

namespace TestTask_ChatApp.Data.Models
{
    public class SentimentResult
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string Label { get; set; } = string.Empty; // e.g., "positive", "negative", "neutral"
        public double ConfidenceScore { get; set; }
        public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

        public ChatMessage Message { get; set; }
    }
}
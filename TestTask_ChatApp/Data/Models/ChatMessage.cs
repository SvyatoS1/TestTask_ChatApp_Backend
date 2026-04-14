namespace TestTask_ChatApp.Data.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; 
        public string Room { get; set; } = "general";
        public DateTime SentAt { get; set; }

        public SentimentResult? Sentiment { get; set; }

    }
}

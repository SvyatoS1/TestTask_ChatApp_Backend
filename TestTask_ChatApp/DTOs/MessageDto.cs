namespace TestTask_ChatApp.DTOs
{
    public record MessageDto(
        int Id,
        string Username,
        string Content,
        string Room,
        DateTime SentAt,
        SentimentDto? Sentiment
    );

    public record SentimentDto(
        string Label,
        double ConfidenceScore
    );

    public record SentimentUpdateDto(
        int MessageId,
        string Label,
        double ConfidenceScore
    );
}

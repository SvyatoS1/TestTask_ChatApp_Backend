namespace TestTask_ChatApp.Services
{
    public interface ISentimentService
    {
        Task<(string Label, double ConfidenceScore)> AnalyzeSentimentAsync(string text);
    }
}

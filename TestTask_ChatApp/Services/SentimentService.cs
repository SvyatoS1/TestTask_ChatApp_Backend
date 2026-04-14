using Azure;
using Azure.AI.TextAnalytics;

namespace TestTask_ChatApp.Services
{
    public class SentimentService : ISentimentService
    {
        private readonly TextAnalyticsClient _client;

        public SentimentService(IConfiguration config)
        {
            var endpoint = config["AzureLanguage:Endpoint"]
                ?? throw new InvalidOperationException("AzureLanguage:Endpoint is not configured.");
            var key = config["AzureLanguage:Key"]
                ?? throw new InvalidOperationException("AzureLanguage:Key is not configured.");

            _client = new TextAnalyticsClient(new Uri(endpoint), new AzureKeyCredential(key));
        }
        public async Task<(string Label, double ConfidenceScore)> AnalyzeSentimentAsync(string text)
        {
            DocumentSentiment result = await _client.AnalyzeSentimentAsync(text);

            var label = result.Sentiment.ToString().ToLower();

            var score = label switch
            {
                "positive" => result.ConfidenceScores.Positive,
                "negative" => result.ConfidenceScores.Negative,
                _ => result.ConfidenceScores.Neutral
            };

            if (label == "mixed")
                label = score == result.ConfidenceScores.Positive ? "positive" : "negative";

            return (label, Math.Round(score,2));
        }
    }
}

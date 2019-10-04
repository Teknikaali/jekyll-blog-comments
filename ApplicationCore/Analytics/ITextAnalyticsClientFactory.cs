using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;

namespace ApplicationCore.Analytics
{
    public interface ITextAnalyticsClientFactory
    {
        ITextAnalyticsClient CreateClient(string subscriptionKey, string region);
    }
}

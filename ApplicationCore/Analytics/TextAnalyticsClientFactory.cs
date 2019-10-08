using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;

namespace ApplicationCore.Analytics
{
    public class TextAnalyticsClientFactory : ITextAnalyticsClientFactory
    {
        private readonly ICredentialsFactory _credentialsFactory;

        public TextAnalyticsClientFactory(ICredentialsFactory credentialsFactory)
        {
            _credentialsFactory = credentialsFactory;
        }

        public ITextAnalyticsClient CreateClient(string subscriptionKey, string region)
        {
            return new TextAnalyticsClient(
                _credentialsFactory.CreateApiKeyServiceCredentials(subscriptionKey))
            {
                Endpoint = $"https://{region}.api.cognitive.microsoft.com"
            };
        }
    }
}

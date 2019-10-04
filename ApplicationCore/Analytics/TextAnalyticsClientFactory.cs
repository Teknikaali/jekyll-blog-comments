using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Rest;

namespace ApplicationCore.Analytics
{
    public class TextAnalyticsClientFactory : ITextAnalyticsClientFactory
    {
        public ITextAnalyticsClient CreateClient(string subscriptionKey, string region)
        {
            return new TextAnalyticsClient(new ApiKeyServiceClientCredentials(subscriptionKey))
            {
                Endpoint = $"https://{region}.api.cognitive.microsoft.com"
            };
        }

        private class ApiKeyServiceClientCredentials : ServiceClientCredentials
        {
            private readonly string _subscriptionKey;

            public ApiKeyServiceClientCredentials(string subscriptionKey)
            {
                _subscriptionKey = subscriptionKey;
            }

            public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken ct)
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", _subscriptionKey);
                return base.ProcessHttpRequestAsync(request, ct);
            }
        }
    }
}

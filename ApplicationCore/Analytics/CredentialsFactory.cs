using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Rest;

namespace ApplicationCore.Analytics
{
    public class CredentialsFactory : ICredentialsFactory
    {
        public ServiceClientCredentials CreateApiKeyServiceCredentials(string subscriptionKey)
        {
            return new ApiKeyServiceClientCredentials(subscriptionKey);
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

using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Analytics;
using Xunit;

namespace ApplicationCore.Tests.Analytics
{
    public class CredentialsFactoryTests
    {
        [Fact]
        public async Task CreatedCredentialsHaveSubscriptionKeyInRequestHeader()
        {
            var subscriptionKey = "SubscriptionKey";
            var factory = new CredentialsFactory();

            var apiKeyService = factory.CreateApiKeyServiceCredentials(subscriptionKey);

            var httpRequest = new HttpRequestMessage();
            await apiKeyService.ProcessHttpRequestAsync(httpRequest, new CancellationToken()).ConfigureAwait(false);

            var enumerator = httpRequest.Headers.GetEnumerator();
            enumerator.MoveNext();

            Assert.Equal(subscriptionKey, enumerator.Current.Value.First());

            httpRequest.Dispose();
        }
    }
}

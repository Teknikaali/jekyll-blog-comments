using System;
using ApplicationCore.Analytics;
using Microsoft.Rest;
using Moq;
using Xunit;

namespace ApplicationCore.Tests.Analytics
{
    public class TextAnalyticsClientFactoryTests
    {
        [Fact]
        public void CreatesRegionalClient()
        {
            var credentialsFactoryMock = new Mock<ICredentialsFactory>();
            credentialsFactoryMock.Setup(x => x.CreateApiKeyServiceCredentials(It.IsAny<string>()))
                .Returns(Mock.Of<ServiceClientCredentials>());

            var clientFactory = new TextAnalyticsClientFactory(credentialsFactoryMock.Object);

            var region = "Region";
            var client = clientFactory.CreateClient("SubscriptionKey", region);

            Assert.Contains(region, client.Endpoint, StringComparison.InvariantCulture);
        }
    }
}

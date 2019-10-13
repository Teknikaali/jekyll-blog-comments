using System;
using Moq;
using Xunit;

namespace ApplicationCore.Tests.Services
{
    public class GitHubClientFactoryTests
    {
        [Fact]
        public void ThrowsIfConfigIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new GitHubClientFactory(null!));
        }

        [Fact]
        public void CreatesClient()
        {
            var configMock = new Mock<IWebConfigurator>();
            configMock.Setup(x => x.GitHubToken)
                .Returns("GitHubTestToken");
            var factory = new GitHubClientFactory(configMock.Object);

            Assert.NotNull(factory.CreateClient());
        }

        [Fact]
        public void CreateClientThrowsWhenCalledWithoutGitHubToken()
        {
            var configMock = new Mock<IWebConfigurator>();
            var factory = new GitHubClientFactory(configMock.Object);

            Assert.Throws<ArgumentNullException>(() => factory.CreateClient());
        }
    }
}

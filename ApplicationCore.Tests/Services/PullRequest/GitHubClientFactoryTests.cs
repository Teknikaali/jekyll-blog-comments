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
            var factory = new GitHubClientFactory(new GitHubConfig { Token = "GitHubTestToken" });

            Assert.NotNull(factory.CreateClient());
        }

        [Fact]
        public void CreateClientThrowsWhenCalledWithoutGitHubToken()
        {
            var factory = new GitHubClientFactory(new GitHubConfig());

            Assert.Throws<ArgumentException>(() => factory.CreateClient());
        }
    }
}

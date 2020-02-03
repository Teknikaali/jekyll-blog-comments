using System;
using Moq;
using Octokit;
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
        public void UsesAnonymousCredentialsWhenGitHubTokenIsNotAvailable()
        {
            var factory = new GitHubClientFactory(new GitHubConfig());

            var client = factory.CreateClient();

            Assert.Equal(Credentials.Anonymous, client.Connection.Credentials);
        }
    }
}

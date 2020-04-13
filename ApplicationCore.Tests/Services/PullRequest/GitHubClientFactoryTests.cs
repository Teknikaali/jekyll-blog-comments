using System;
using Microsoft.Extensions.Options;
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
            var factory = new GitHubClientFactory(
                Options.Create(new WebConfiguration { GitHubToken = "GitHubTestToken" }));

            Assert.NotNull(factory.CreateClient());
        }

        [Fact]
        public void UsesAnonymousCredentialsWhenGitHubTokenIsNotAvailable()
        {
            var factory = new GitHubClientFactory(Options.Create(new WebConfiguration()));

            var client = factory.CreateClient();

            Assert.Equal(Credentials.Anonymous, client.Connection.Credentials);
        }
    }
}

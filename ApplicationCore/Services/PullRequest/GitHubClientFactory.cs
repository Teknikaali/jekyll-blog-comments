using System;
using Octokit;

namespace ApplicationCore
{
    public class GitHubClientFactory : IGitHubClientFactory
    {
        private readonly GitHubConfig _config;

        public GitHubClientFactory(GitHubConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IGitHubClient CreateClient()
        {
            var credentials = _config.Token.Length > 0
                ? new Credentials(_config.Token)
                : Credentials.Anonymous;

            return new GitHubClient(
                new ProductHeaderValue("PostCommentToPullRequest"),
                new Octokit.Internal.InMemoryCredentialStore(credentials));
        }
    }
}

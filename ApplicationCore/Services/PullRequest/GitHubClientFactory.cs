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
            return new GitHubClient(
                new ProductHeaderValue("PostCommentToPullRequest"),
                    new Octokit.Internal.InMemoryCredentialStore(
                        new Credentials(_config.Token)));
        }
    }
}

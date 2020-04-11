using System;
using Octokit;

namespace ApplicationCore
{
    public class GitHubClientFactory : IGitHubClientFactory
    {
        private readonly WebConfiguration _config;

        public GitHubClientFactory(WebConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IGitHubClient CreateClient()
        {
            var credentials = _config.GitHubToken.Length > 0
                ? new Credentials(_config.GitHubToken)
                : Credentials.Anonymous;

            return new GitHubClient(
                new ProductHeaderValue("PostCommentToPullRequest"),
                new Octokit.Internal.InMemoryCredentialStore(credentials));
        }
    }
}

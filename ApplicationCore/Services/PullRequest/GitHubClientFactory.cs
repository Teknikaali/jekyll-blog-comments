using Octokit;

namespace ApplicationCore
{
    public class GitHubClientFactory : IGitHubClientFactory
    {
        private readonly IWebConfigurator _config;

        public GitHubClientFactory(IWebConfigurator config)
        {
            _config = config;
        }

        public IGitHubClient CreateClient()
        {
            return new GitHubClient(
                new ProductHeaderValue("PostCommentToPullRequest"),
                    new Octokit.Internal.InMemoryCredentialStore(
                        new Credentials(_config.GitHubToken)));
        }
    }
}

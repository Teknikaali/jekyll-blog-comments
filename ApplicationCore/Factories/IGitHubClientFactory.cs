using Octokit;

namespace ApplicationCore
{
    public interface IGitHubClientFactory
    {
        IGitHubClient CreateClient();
    }
}

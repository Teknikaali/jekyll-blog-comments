using Octokit;

namespace ApplicationCore
{
    /// <summary>
    /// Enables creating a GitHub client.
    /// </summary>
    public interface IGitHubClientFactory
    {
        /// <summary>
        /// Create the GitHub client.
        /// </summary>
        /// <returns><seealso cref="IGitHubClient"/>The GitHub client.</returns>
        IGitHubClient CreateClient();
    }
}

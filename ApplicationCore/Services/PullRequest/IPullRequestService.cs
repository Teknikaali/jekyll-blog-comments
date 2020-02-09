using System.Threading.Tasks;
using ApplicationCore.Model;

namespace ApplicationCore
{
    /// <summary>
    /// Enables creating a pull request using the comment
    /// </summary>
    public interface IPullRequestService
    {
        /// <summary>
        /// Try create a pull request containing the <seealso cref="Comment"/>.
        /// </summary>
        /// <param name="comment">Comment to create teh pull request with.</param>
        /// <returns>Result of the pull request.</returns>
        Task<PullRequestResult> TryCreatePullRequestAsync(Comment comment);
    }
}

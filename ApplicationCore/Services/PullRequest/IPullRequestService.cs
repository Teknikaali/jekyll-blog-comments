using System.Threading.Tasks;
using ApplicationCore.Model;

namespace ApplicationCore
{
    public interface IPullRequestService
    {
        Task<PullRequestResult> TryCreatePullRequestAsync(Comment comment);
    }
}

using System.Threading.Tasks;
using ApplicationCore.Model;
using Microsoft.Extensions.Logging;

namespace ApplicationCore
{
    public class NoOpPullRequestService : IPullRequestService
    {
        private readonly ILogger _log;

        public NoOpPullRequestService(ILoggerProvider loggerProvider)
        {
            if (loggerProvider is null)
            {
                throw new System.ArgumentNullException(nameof(loggerProvider));
            }

            _log = loggerProvider.CreateLogger(nameof(NoOpPullRequestService));
        }

        public Task<PullRequestResult> TryCreatePullRequestAsync(Comment comment)
        {
            _log.LogInformation(CommentResources.NoOpPullRequestSkipped);

            return Task.FromResult(new PullRequestResult());
        }
    }
}

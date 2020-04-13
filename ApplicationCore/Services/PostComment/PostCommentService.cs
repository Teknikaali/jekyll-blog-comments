using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using ApplicationCore.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ApplicationCore
{
    public class PostCommentService : IPostCommentService
    {
        private readonly WebConfiguration _config;
        private readonly ICommentFactory _commentFactory;
        private readonly IPullRequestService _pullRequestService;

        public PostCommentService(
            IOptions<WebConfiguration> config,
            ICommentFactory commentFactory,
            IPullRequestService pullRequestService)
        {
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _commentFactory = commentFactory ?? throw new ArgumentNullException(nameof(commentFactory));
            _pullRequestService = pullRequestService ?? throw new ArgumentNullException(nameof(pullRequestService));
        }

        public async Task<PostCommentResult> PostCommentAsync(IFormCollection form)
        {
            if (form is null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            // Make sure the site posting the comment is the correct site.
            var allowedSite = _config.Website;
            var postedSite = form[FormFields.CommentSite];
            if (!string.IsNullOrWhiteSpace(allowedSite) && !AreSameSites(allowedSite, postedSite))
            {
                return new PostCommentResult(
                    HttpStatusCode.BadRequest,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        CommentResources.AreNotSameSitesErrorMessage,
                        postedSite.ToString() ?? "null"));
            }

            var commentResult = await _commentFactory.CreateFromFormAsync(form).ConfigureAwait(false);
            if (commentResult.HasErrors)
            {
                return new PostCommentResult(
                 HttpStatusCode.BadRequest,
                 $"Creating comment from form failed: {string.Join("\n", commentResult.Errors)}",
                 commentResult.Exception);
            }

            var pullRequestResult = await _pullRequestService.TryCreatePullRequestAsync(commentResult.Comment)
                .ConfigureAwait(false);
            if (pullRequestResult.HasError)
            {
                return new PostCommentResult(
                    HttpStatusCode.BadRequest,
                    $"Creating pull request failed: {pullRequestResult.Error}",
                    pullRequestResult.Exception);
            }

            if (!Uri.TryCreate(form[FormFields.Redirect], UriKind.Absolute, out var redirectUri))
            {
                return new PostCommentResult(HttpStatusCode.OK);
            }
            else
            {
                return new PostCommentResult(HttpStatusCode.Redirect, redirectUri);
            }
        }

        private static bool AreSameSites(string commentSite, string postedCommentSite)
        {
            return Uri.TryCreate(commentSite, UriKind.Absolute, out var commentSiteUri)
                && Uri.TryCreate(postedCommentSite, UriKind.Absolute, out var postedCommentSiteUri)
                && commentSiteUri.Host.Equals(postedCommentSiteUri.Host, StringComparison.OrdinalIgnoreCase);
        }
    }
}

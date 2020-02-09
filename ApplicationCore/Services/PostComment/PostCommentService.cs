﻿using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using ApplicationCore.Model;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore
{
    public class PostCommentService : IPostCommentService
    {
        private readonly CommentConfig _config;
        private readonly ICommentFactory _commentFactory;
        private readonly IPullRequestService _pullRequestService;

        public PostCommentService(
            CommentConfig config,
            ICommentFactory commentFactory,
            IPullRequestService pullRequestService)
        {
            _config = config;
            _commentFactory = commentFactory;
            _pullRequestService = pullRequestService;
        }

        public async Task<PostCommentResult> PostCommentAsync(IFormCollection form)
        {
            if (form is null)
            {
                throw new ArgumentNullException(nameof(form));
            }

            // Make sure the site posting the comment is the correct site.
            var allowedSite = _config.WebsiteUrl.AbsoluteUri;
            var postedSite = form["CommentSite"]; // TODO: fix magic string
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

            if (!commentResult.HasErrors)
            {
                var pullRequestResult = await _pullRequestService.TryCreatePullRequestAsync(commentResult.Comment).ConfigureAwait(false);

                if (!pullRequestResult.HasError)
                {
                    if (!Uri.TryCreate(form["Redirect"], UriKind.Absolute, out var redirectUri))
                    {
                        return new PostCommentResult(HttpStatusCode.OK);
                    }
                    else
                    {
                        return new PostCommentResult(HttpStatusCode.Redirect, redirectUri);
                    }
                }
                else
                {
                    return new PostCommentResult(HttpStatusCode.BadRequest, $"Creating pull request failed: {pullRequestResult.Error}");
                }
            }
            else
            {
                // TODO: Include possible exception
                return new PostCommentResult(HttpStatusCode.BadRequest, string.Join("\n", commentResult.Errors)); 
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

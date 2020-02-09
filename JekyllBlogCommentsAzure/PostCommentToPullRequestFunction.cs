using System;
using System.Net;
using System.Threading.Tasks;
using ApplicationCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace JekyllBlogCommentsAzure
{
    /// <summary>
    /// Azure Function that posts input form as a comment to a repository.
    /// </summary>
    public class PostCommentToPullRequestFunction
    {
        private readonly IPostCommentService _postCommentService;

        /// <summary>
        /// Create new Function that posts comments
        /// </summary>
        /// <param name="postCommentService">Comment posting service. This does the actual work.</param>
        public PostCommentToPullRequestFunction(IPostCommentService postCommentService)
        {
            _postCommentService = postCommentService;
        }

        [FunctionName("PostComment")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request,
            ILogger logger)
        {
            if (request is null)
            {
                logger.LogError(CommentResources.ParameterCannotBeNull, nameof(request));
                return new BadRequestResult();
            }

            try
            {
                var form = await request.ReadFormAsync().ConfigureAwait(false);
                var result = await _postCommentService.PostCommentAsync(form).ConfigureAwait(false);

                switch (result.HttpStatusCode)
                {
                    case HttpStatusCode.OK:
                        return new OkResult();
                    case HttpStatusCode.Redirect:
                        return new RedirectResult(result.RedirectUrl?.AbsoluteUri);
                    default:
                        logger.LogError(result.Exception, result.Error);
                        return new BadRequestResult();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, CommentResources.PostCommentExceptionMessage, e.Message);
                throw;
            }
        }
    }
}

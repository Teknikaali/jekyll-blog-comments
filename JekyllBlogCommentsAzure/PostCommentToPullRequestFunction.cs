using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ApplicationCore;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace JekyllBlogCommentsAzure
{
    public class PostCommentToPullRequestFunction
    {
        private readonly IPostCommentService _postCommentService;

        public PostCommentToPullRequestFunction(IPostCommentService postCommentService)
        {
            _postCommentService = postCommentService;
        }

        [FunctionName("PostComment")]
        public async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage request)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var form = await request.Content.ReadAsFormDataAsync().ConfigureAwait(false);

            PostCommentResult result;

            try
            {
                result = await _postCommentService.PostCommentAsync(form).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                result = new PostCommentResult(HttpStatusCode.BadRequest, $"Posting comment failed: {e.Message}", e);
            }

            switch (result.HttpStatusCode)
            {
                case HttpStatusCode.OK:
                    return request.CreateResponse(HttpStatusCode.OK);
                case HttpStatusCode.Redirect:
                    {
                        var response = request.CreateResponse(HttpStatusCode.Redirect);
                        response.Headers.Location = result.RedirectUrl;
                        return response;
                    }
                default:
                    {
                        if(result.Exception != null)
                        {
                            return request.CreateErrorResponse(result.HttpStatusCode, new HttpError(result.Exception, includeErrorDetail: true) { Message = result.Error });
                        }
                        else
                        {
                            return request.CreateErrorResponse(result.HttpStatusCode, result.Error);
                        }
                    }
            }
        }
    }
}

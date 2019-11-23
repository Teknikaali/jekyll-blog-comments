using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ApplicationCore;
using Moq;
using Xunit;

namespace JekyllBlogCommentsAzure.Tests
{
    public class PostCommentToPullRequestFunctionTests
    {
        [Fact]
        public void ThrowsWhenRequestIsNull()
        {
            var serviceMock = new Mock<IPostCommentService>();
            var function = new PostCommentToPullRequestFunction(serviceMock.Object);

            Assert.ThrowsAsync<ArgumentNullException>(() => function.Run(null!));
        }

        [Fact]
        public async Task ReturnsOkIfPostingCommentSucceeds()
        {
            var serviceMock = new Mock<IPostCommentService>();
            serviceMock.Setup(x => x.PostCommentAsync(It.IsAny<NameValueCollection>()))
                .ReturnsAsync(new PostCommentResult(HttpStatusCode.OK));
            var function = new PostCommentToPullRequestFunction(serviceMock.Object);

            HttpResponseMessage response;
            
            using(var stream = new MemoryStream())
            using(var content = new StreamContent(stream))
            using(var request = new HttpRequestMessage { Content = content })
            {
                response = await function.Run(request).ConfigureAwait(false);
            }

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}

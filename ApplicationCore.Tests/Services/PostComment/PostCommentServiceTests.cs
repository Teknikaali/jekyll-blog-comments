using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using ApplicationCore.Model;
using Moq;
using Xunit;

namespace ApplicationCore.Tests.Services
{
    public class PostCommentServiceTests
    {
        private const string _website = "http://www.example.com/";
        private readonly CommentConfig _config;

        public PostCommentServiceTests()
        {
            _config = new CommentConfig
            {
                WebsiteUrl = new Uri(_website)
            };
        }

        [Fact]
        public async Task ThrowsIfFormIsNull()
        {
            var postCommentService = new PostCommentService(
                new CommentConfig(),
                Mock.Of<ICommentFactory>(),
                Mock.Of<IPullRequestService>());

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => postCommentService.PostCommentAsync(null!)).ConfigureAwait(false);
        }

        [Theory]
        [InlineData(null!)]
        [InlineData("")]
        [InlineData("http://www.example.org")]
        public async Task ReturnsErrorResultIfCommentDoesNotComeFromSpecifiedWebsite(string postedSite)
        {
            var postCommentService = new PostCommentService(
                _config,
                Mock.Of<ICommentFactory>(),
                Mock.Of<IPullRequestService>());

            var errorResult = await postCommentService.PostCommentAsync(new NameValueCollection
            {
                { "CommentSite", postedSite }
            }).ConfigureAwait(false);

            Assert.NotEmpty(errorResult.Error);
            Assert.Equal(HttpStatusCode.BadRequest, errorResult.HttpStatusCode);
        }

        [Fact]
        public async Task ReturnsErrorResultIfFormHasErrors()
        {
            var errorMessage = "ErrorBecauseOfReasons";
            var commentFactoryMock = new Mock<ICommentFactory>();
            commentFactoryMock.Setup(x => x.CreateFromFormAsync(It.IsAny<NameValueCollection>()))
                .ReturnsAsync(new CommentResult(new Comment(string.Empty, string.Empty, string.Empty), new string[]
                {
                    errorMessage
                }));

            var postCommentService = new PostCommentService(
               _config,
               commentFactoryMock.Object,
               Mock.Of<IPullRequestService>());

            var errorResult = await postCommentService.PostCommentAsync(new NameValueCollection
            {
                { "CommentSite", _website }
            }).ConfigureAwait(false);

            Assert.NotEmpty(errorResult.Error);
            Assert.Equal(HttpStatusCode.BadRequest, errorResult.HttpStatusCode);
            Assert.Equal(errorMessage, errorResult.Error);
        }

        [Fact]
        public async Task ReturnsErrorResultIfPullRequestFails()
        {
            var commentFactoryMock = new Mock<ICommentFactory>();
            commentFactoryMock.Setup(x => x.CreateFromFormAsync(It.IsAny<NameValueCollection>()))
                .ReturnsAsync(new CommentResult(new Comment(string.Empty, string.Empty, string.Empty)));

            var errorMessage = "ErrorBecauseOfReasons";
            var pullRequestServiceMock = new Mock<IPullRequestService>();
            pullRequestServiceMock.Setup(x => x.TryCreatePullRequestAsync(It.IsAny<Comment>()))
                .ReturnsAsync(new PullRequestResult(errorMessage));

            var postCommentService = new PostCommentService(
               _config,
               commentFactoryMock.Object,
               pullRequestServiceMock.Object);

            var errorResult = await postCommentService.PostCommentAsync(new NameValueCollection
            {
                { "CommentSite", _website }
            }).ConfigureAwait(false);

            Assert.NotEmpty(errorResult.Error);
            Assert.Equal(HttpStatusCode.BadRequest, errorResult.HttpStatusCode);
            Assert.Contains(errorMessage, errorResult.Error, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ReturnsOkresultIfNoRedirectUriIsSpecified()
        {
            var commentFactoryMock = new Mock<ICommentFactory>();
            commentFactoryMock.Setup(x => x.CreateFromFormAsync(It.IsAny<NameValueCollection>()))
                .ReturnsAsync(new CommentResult(new Comment(string.Empty, string.Empty, string.Empty)));

            var pullRequestServiceMock = new Mock<IPullRequestService>();
            pullRequestServiceMock.Setup(x => x.TryCreatePullRequestAsync(It.IsAny<Comment>()))
                .ReturnsAsync(new PullRequestResult());

            var postCommentService = new PostCommentService(
               _config,
               commentFactoryMock.Object,
               pullRequestServiceMock.Object);

            var okResult = await postCommentService.PostCommentAsync(new NameValueCollection
            {
                { "CommentSite", _website }
            }).ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.OK, okResult.HttpStatusCode);
            Assert.Empty(okResult.Error);
        }

        [Fact]
        public async Task ReturnsRedirectResultIfRedirectIsSpecified()
        {
            var commentFactoryMock = new Mock<ICommentFactory>();
            commentFactoryMock.Setup(x => x.CreateFromFormAsync(It.IsAny<NameValueCollection>()))
                .ReturnsAsync(new CommentResult(new Comment(string.Empty, string.Empty, string.Empty)));

            var pullRequestServiceMock = new Mock<IPullRequestService>();
            pullRequestServiceMock.Setup(x => x.TryCreatePullRequestAsync(It.IsAny<Comment>()))
                .ReturnsAsync(new PullRequestResult());

            var postCommentService = new PostCommentService(
               _config,
               commentFactoryMock.Object,
               pullRequestServiceMock.Object);

            var redirectResult = await postCommentService.PostCommentAsync(new NameValueCollection
            {
                { "CommentSite", _website },
                { "Redirect", _website }
            }).ConfigureAwait(false);

            Assert.Empty(redirectResult.Error);
            Assert.Equal(HttpStatusCode.Redirect, redirectResult.HttpStatusCode);
            Assert.Equal(_website, redirectResult.RedirectUrl!.AbsoluteUri);
        }
    }
}

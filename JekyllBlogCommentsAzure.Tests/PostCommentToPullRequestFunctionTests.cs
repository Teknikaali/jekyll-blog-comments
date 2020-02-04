using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace JekyllBlogCommentsAzure.Tests
{
    public class PostCommentToPullRequestFunctionTests
    {
        [Fact]
        public async Task LogsAndReturnsBadRequestIfRequestIsNull()
        {
            var serviceMock = new Mock<IPostCommentService>();
            var function = new PostCommentToPullRequestFunction(serviceMock.Object);
            var loggerMock = new Mock<ILogger>();

            var result = await function.Run(null!, loggerMock.Object).ConfigureAwait(false);
            Assert.IsType<BadRequestResult>(result);
            loggerMock.Verify(x => x.Log(
                    LogLevel.Error,
                    0,
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()));
        }

        [Fact]
        public async Task ReturnsOkIfPostingCommentSucceeds()
        {
            var serviceMock = new Mock<IPostCommentService>();
            serviceMock.Setup(x => x.PostCommentAsync(It.IsAny<IFormCollection>()))
                .ReturnsAsync(new PostCommentResult(HttpStatusCode.OK));
            var function = new PostCommentToPullRequestFunction(serviceMock.Object);

            var response = await function.Run(Mock.Of<HttpRequest>(), Mock.Of<ILogger>()).ConfigureAwait(false);

            Assert.IsType<OkResult>(response);
        }

        [Theory]
        [MemberData(nameof(FailingCommentExceptions))]
        public async Task LogsAndReturnsBadRequestIfPostingCommentFails(Exception exception)
        {
            var serviceMock = new Mock<IPostCommentService>();
            serviceMock.Setup(x => x.PostCommentAsync(It.IsAny<IFormCollection>()))
                .ReturnsAsync(new PostCommentResult(HttpStatusCode.BadRequest, exception?.Message!, exception!));

            var formMock = new Mock<IFormCollection>();
            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.ReadFormAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(formMock.Object);
            var loggerMock = new Mock<ILogger>();
            var function = new PostCommentToPullRequestFunction(serviceMock.Object);

            var result = await function.Run(requestMock.Object, loggerMock.Object).ConfigureAwait(false);
            
            Assert.IsType<BadRequestResult>(result);
            loggerMock.Verify(x =>
                x.Log(LogLevel.Error, 0, It.IsAny<object>(), exception, It.IsAny<Func<object, Exception, string>>()));
        }

        [Fact]
        public async Task ReturnsRedirectIfPostingCommentSucceedsWithRedirection()
        {
            var redirectUri = new Uri("http://www.example.com/");
            var serviceMock = new Mock<IPostCommentService>();
            serviceMock.Setup(x => x.PostCommentAsync(It.IsAny<IFormCollection>()))
                .ReturnsAsync(new PostCommentResult(HttpStatusCode.Redirect, redirectUri));
            var function = new PostCommentToPullRequestFunction(serviceMock.Object);

            var response = await function.Run(Mock.Of<HttpRequest>(), Mock.Of<ILogger>()).ConfigureAwait(false);

            Assert.IsType<RedirectResult>(response);
            Assert.Equal(redirectUri.AbsoluteUri, ((RedirectResult)response).Url);
        }

        [Fact]
        public async Task ExceptionsAreRethrown()
        {
            var function = new PostCommentToPullRequestFunction(null!);

            var exception = await Assert.ThrowsAsync<NullReferenceException>(
                () => function.Run(Mock.Of<HttpRequest>(), Mock.Of<ILogger>())).ConfigureAwait(false);
        }

        public static IEnumerable<object[]> FailingCommentExceptions =>
            new List<object[]>
            {
                new object[] { new ArgumentException(string.Empty) },
                new object[] { new ArgumentNullException(nameof(FailingCommentExceptions)) },
                new object[] { new InvalidOperationException() },
                new object[] { new NullReferenceException() }
            };
    }
}

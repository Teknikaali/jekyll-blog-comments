using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ApplicationCore.Tests.Services
{
    public class NoOpPullRequestTests
    {
        [Fact]
        public void ConstructorThrowsIfLoggerIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new NoOpPullRequestService(null!));
        }

        [Fact]
        public async Task CreatePullRequestReturnsValidResult()
        {
            var loggerMock = new Mock<ILogger>();
            var loggerProviderMock = new Mock<ILoggerProvider>();
            loggerProviderMock.Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(loggerMock.Object);

            var noOpPullRequestService = new NoOpPullRequestService(loggerProviderMock.Object);
            var result = await noOpPullRequestService.TryCreatePullRequestAsync(null!).ConfigureAwait(false);

            Assert.False(result.HasError);
        }
    }
}

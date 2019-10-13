using System;
using System.Threading.Tasks;
using ApplicationCore.Model;
using Moq;
using Octokit;
using Xunit;
using YamlDotNet.Serialization;

namespace ApplicationCore.Tests.Services
{
    public class PullRequestServiceTests
    {
        private const string _pullRequestRepository = "Test/Repository";

        [Fact]
        public async Task ThrowsIfCommentIsNull()
        {
            var pullRequestService = new PullRequestService(
                Mock.Of<IWebConfigurator>(),
                Mock.Of<ISerializer>(),
                Mock.Of<IGitHubClient>());

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => pullRequestService.TryCreatePullRequestAsync(null!))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task ReturnsErrorResultIfNoRepositoryFound()
        {
            var configMock = new Mock<IWebConfigurator>();
            configMock.Setup(x => x.PullRequestRepository)
                .Returns(_pullRequestRepository);

            var gitHubClientMock = new Mock<IGitHubClient>();
            gitHubClientMock.Setup(x => x.Repository.Get(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<ApiException>();

            var pullRequestService = new PullRequestService(
                configMock.Object,
                Mock.Of<ISerializer>(),
                gitHubClientMock.Object);

            var result = await pullRequestService.TryCreatePullRequestAsync(
                new Comment(string.Empty, string.Empty, string.Empty))
                .ConfigureAwait(false);

            Assert.True(result.HasError);
            Assert.NotEmpty(result.Error);
        }

        [Fact]
        public async Task ReturnsEmptyResultIfSuccess()
        {
            var comment = new Comment("test-post-id", "This is a test message", "John Doe");

            var configMock = new Mock<IWebConfigurator>();
            configMock.Setup(x => x.PullRequestRepository)
                .Returns(_pullRequestRepository);

            var gitHubClientMock = new Mock<IGitHubClient>();
            gitHubClientMock.Setup(x => x.Repository.Get(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Repository());
            gitHubClientMock.Setup(x => x.Repository.Branch.Get(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(new Branch("TestBranch", new GitReference("nodeId", "url", "label", "ref", "sha", new User(), new Repository()), false));
            gitHubClientMock.Setup(x => x.Git.Reference.Create(It.IsAny<long>(), It.IsAny<NewReference>()))
                .ReturnsAsync(new Reference("ref", "nodeId", "url", new TagObject()));
            gitHubClientMock.Setup(x => x.Repository.Content.CreateFile(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CreateFileRequest>()))
                .ReturnsAsync(new RepositoryContentChangeSet());
            gitHubClientMock.Setup(x => x.Repository.PullRequest.Create(It.IsAny<long>(), It.IsAny<NewPullRequest>()))
                .ReturnsAsync(new PullRequest());

            var serializerMock = new Mock<ISerializer>();
            serializerMock.Setup(x => x.Serialize(It.IsAny<object>()))
                .Returns(comment.Message);

            var pullRequestService = new PullRequestService(
                configMock.Object,
                serializerMock.Object,
                gitHubClientMock.Object);

            var result = await pullRequestService.TryCreatePullRequestAsync(comment).ConfigureAwait(false);

            Assert.False(result.HasError);
        }
    }
}

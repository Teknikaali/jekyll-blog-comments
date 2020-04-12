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
                new WebConfiguration(),
                Mock.Of<ISerializerFactory>(),
                Mock.Of<IGitHubClientFactory>());

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => pullRequestService.TryCreatePullRequestAsync(null!))
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task ReturnsErrorResultIfNoRepositoryFound()
        {
            var config = new WebConfiguration
            {
                PullRequestRepository = _pullRequestRepository
            };

            var gitHubClientMock = new Mock<IGitHubClient>();
            var gitHubClientFactoryMock = new Mock<IGitHubClientFactory>();
            gitHubClientFactoryMock.Setup(x => x.CreateClient())
                .Returns(gitHubClientMock.Object);
            gitHubClientMock.Setup(x => x.Repository.Get(It.IsAny<string>(), It.IsAny<string>()))
                .Throws<ApiException>();

            var pullRequestService = new PullRequestService(
                config,
                Mock.Of<ISerializerFactory>(),
                gitHubClientFactoryMock.Object);

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

            var config = new WebConfiguration
            {
                PullRequestRepository = _pullRequestRepository,
                Website = "http://www.example.com",
                FallbackCommitEmail = "redacted@example.com"
            };

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
            var gitHubClientFactoryMock = new Mock<IGitHubClientFactory>();
            gitHubClientFactoryMock.Setup(x => x.CreateClient())
                .Returns(gitHubClientMock.Object);

            var serializerMock = new Mock<ISerializer>();
            serializerMock.Setup(x => x.Serialize(It.IsAny<object>()))
                .Returns(comment.Message);
            var serializerFactoryMock = new Mock<ISerializerFactory>();
            serializerFactoryMock.Setup(x => x.BuildSerializer()).Returns(serializerMock.Object);

            var pullRequestService = new PullRequestService(
                config,
                serializerFactoryMock.Object,
                gitHubClientFactoryMock.Object);

            var result = await pullRequestService.TryCreatePullRequestAsync(comment).ConfigureAwait(false);

            Assert.False(result.HasError);
        }

        [Fact]
        public async Task ReturnsErrorResultIfRepositoryNotFound()
        {
            var comment = new Comment("postId", "message", "name");

            var gitHubClientMock = new Mock<IGitHubClient>();
            gitHubClientMock.Setup(x => x.Repository.Get(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Repository());

            var gitHubClientFactoryMock = new Mock<IGitHubClientFactory>();
            gitHubClientFactoryMock.Setup(x => x.CreateClient())
                .Returns(gitHubClientMock.Object);

            var serializerMock = new Mock<ISerializer>();
            serializerMock.Setup(x => x.Serialize(It.IsAny<object>()))
                .Returns(comment.Message);

            var serializerFactoryMock = new Mock<ISerializerFactory>();
            serializerFactoryMock.Setup(x => x.BuildSerializer()).Returns(serializerMock.Object);

            var pullRequestService = new PullRequestService(
                new WebConfiguration
                {
                    PullRequestRepository = "Test"
                },
                serializerFactoryMock.Object,
                gitHubClientFactoryMock.Object);

            var result = await pullRequestService.TryCreatePullRequestAsync(comment).ConfigureAwait(false);

            Assert.True(result.HasError);
            Assert.IsType<IndexOutOfRangeException>(result.Exception);
        }
    }
}

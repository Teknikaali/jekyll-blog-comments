using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ApplicationCore.Analytics;
using ApplicationCore.Model;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Rest;
using Moq;
using Xunit;

namespace ApplicationCore.Tests.Analytics
{
    public class TextAnalyzerTests
    {
        private const string _validSubscriptionKey = "TotallyValidSubscriptionKey";
        private const string _invalidSubscriptionKey = "";

        [Theory]
        [MemberData(nameof(SentimentScoreTestCases))]
        public async Task AnalyzesSentimentScore(string message, double? score)
        {
            var config = new WebConfiguration
            {
                TextAnalyticsSubscriptionKey = _validSubscriptionKey
            };

            // SentimentBatchAsync extension method uses SentimentWithHttpMessagesAsync internally and mocking
            // extension methods with Moq isn't currently possible. Extension methods are "hard dependencies" that 
            // one could also break by introducing a wrapper class and an interface.
            // Let's mock SentimentWithHttpMessagesAsync method here instead.
            var httpOperationResponse = new HttpOperationResponse<SentimentBatchResult>
            {
                Body = new SentimentBatchResult(new SentimentBatchResultItem[]
                {
                    new SentimentBatchResultItem(score: score)
                })
            };
            var textAnalyticsClientMock = new Mock<ITextAnalyticsClient>();
            textAnalyticsClientMock.Setup(x => x.SentimentWithHttpMessagesAsync(
                It.IsAny<bool?>(),
                It.IsAny<MultiLanguageBatchInput>(),
                It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(httpOperationResponse);

            var textAnalyticsClientFactoryMock = new Mock<ITextAnalyticsClientFactory>();
            textAnalyticsClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(textAnalyticsClientMock.Object);
            
            ITextAnalyzer textAnalyzer = new TextAnalyzer(config, textAnalyticsClientFactoryMock.Object);

            var comment = new Comment("test-post-id", message, "John Doe");
            var analyzedComment = await textAnalyzer.AnalyzeAsync(comment).ConfigureAwait(false);

            httpOperationResponse.Dispose();

            Assert.Equal($"{score:0.00}", analyzedComment.Comment.Score);
        }

        public static IEnumerable<object?[]> SentimentScoreTestCases => new List<object?[]>
        {
            new object[] { new string('*', 555), 0.75 },
            new object[] { string.Empty, 0 }
        };

        [Fact]
        public async Task ReturnsCommentIntactIfNotAnalyzed()
        {
            var config = new WebConfiguration
            {
                TextAnalyticsSubscriptionKey = _invalidSubscriptionKey
            };

            var textAnalyticsClientFactoryMock = new Mock<ITextAnalyticsClientFactory>
            {
                DefaultValue = DefaultValue.Mock
            };
            ITextAnalyzer textAnalyzer = new TextAnalyzer(config, textAnalyticsClientFactoryMock.Object);

            var comment = new Comment("test-post-id", new string('*', 555), "John Doe");
            var result = await textAnalyzer.AnalyzeAsync(comment).ConfigureAwait(false);

            Assert.Equal(comment.Score, result.Comment.Score);
        }

        [Theory]
        [InlineData(_invalidSubscriptionKey, false)]
        [InlineData(_validSubscriptionKey, true)]
        public void CanAnalyzeOnlyIfValidSubscriptionKeyIsProvided(string subscriptionKey, bool expectedResult)
        {
            var config = new WebConfiguration
            {
                TextAnalyticsSubscriptionKey = subscriptionKey
            };

            var textAnalyticsClientFactoryMock = new Mock<ITextAnalyticsClientFactory>();
            var textAnalyzer = new TextAnalyzer(config, textAnalyticsClientFactoryMock.Object);

            Assert.Equal(expectedResult, textAnalyzer.CanAnalyze);
        }

        [Fact]
        public void ThrowsIfConstructorDependenciesAreInvalid()
        {
            Assert.Throws<ArgumentNullException>(
                () => new TextAnalyzer(new WebConfiguration(), null!));
            Assert.Throws<ArgumentNullException>(
                () => new TextAnalyzer(null!, Mock.Of<ITextAnalyticsClientFactory>()));
        }

        [Fact]
        public async Task ThrowsIfTryingToAnalyzeNullComment()
        {
            var textAnalyzer = new TextAnalyzer(new WebConfiguration(), Mock.Of<ITextAnalyticsClientFactory>());
            
            await Assert.ThrowsAsync<ArgumentNullException>(() => textAnalyzer.AnalyzeAsync(null!))
                .ConfigureAwait(false);
        }
    }

    public interface IExtendedTextAnalyticsClient : ITextAnalyticsClient
    {
        Task<SentimentBatchResult> SentimentBatchAsync(
            MultiLanguageBatchInput multiLanguageBatchInput,
            bool? showStats,
            CancellationToken ct);
    }
}

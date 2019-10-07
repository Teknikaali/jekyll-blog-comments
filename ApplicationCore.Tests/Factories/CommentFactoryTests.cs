using System.Collections.Specialized;
using System.Threading.Tasks;
using ApplicationCore.Analytics;
using ApplicationCore.Model;
using Moq;
using Xunit;

namespace ApplicationCore.Tests.Factories
{
    public class CommentFactoryTests
    {
        [Fact]
        public async Task ValidCommentIsAnalyzedWhenCanAnalyze()
        {
            var nonAnalyzedComment = new CommentResult(
                new Comment("this-is-non-analyzed-post", "I have not been analyzed", "Non analyzed"));
            var analyzedComment = new CommentResult(
                new Comment("this-is-analyzed-post", "I have been analyzed", "Analyzed"));

            var commentFormMock = new Mock<ICommentForm>();
            commentFormMock.Setup(x => x.HasErrors)
                .Returns(false);
            commentFormMock.Setup(x => x.TryCreateComment())
                .Returns(nonAnalyzedComment);

            var commentFormFactoryMock = new Mock<ICommentFormFactory>();
            commentFormFactoryMock.Setup(x => x.CreateCommentForm(It.IsAny<NameValueCollection>()))
                .Returns(commentFormMock.Object);

            var textAnalyzerMock = new Mock<ITextAnalyzer>();
            textAnalyzerMock.Setup(x => x.CanAnalyze)
                .Returns(true);
            textAnalyzerMock.Setup(x => x.AnalyzeAsync(It.IsAny<Comment>()))
                .ReturnsAsync(analyzedComment);

            ICommentFactory commentFactory = new CommentFactory(
                commentFormFactoryMock.Object,
                textAnalyzerMock.Object);

            var actualComment = await commentFactory.CreateFromFormAsync(new NameValueCollection())
                .ConfigureAwait(false);

            Assert.Equal(analyzedComment, actualComment);
        }

        // TODO: merge with ValidCommentIsAnalyzedWhenCanAnalyze (Theory) and add more cases

        [Fact]
        public async Task InvalidCommentIsNotAnalyzed()
        {
            var nonAnalyzedComment = new CommentResult(
                new Comment("this-is-non-analyzed-post", "I have not been analyzed", "Non analyzed"));
            var analyzedComment = new CommentResult(
                new Comment("this-is-analyzed-post", "I have been analyzed", "Analyzed"));

            var commentFormMock = new Mock<ICommentForm>();
            commentFormMock.Setup(x => x.HasErrors)
                .Returns(true);
            commentFormMock.Setup(x => x.TryCreateComment())
                .Returns(nonAnalyzedComment);

            var commentFormFactoryMock = new Mock<ICommentFormFactory>();
            commentFormFactoryMock.Setup(x => x.CreateCommentForm(It.IsAny<NameValueCollection>()))
                .Returns(commentFormMock.Object);

            var textAnalyzerMock = new Mock<ITextAnalyzer>();
            textAnalyzerMock.Setup(x => x.CanAnalyze)
                .Returns(true);
            textAnalyzerMock.Setup(x => x.AnalyzeAsync(It.IsAny<Comment>()))
                .ReturnsAsync(analyzedComment);

            ICommentFactory commentFactory = new CommentFactory(
                commentFormFactoryMock.Object,
                textAnalyzerMock.Object);

            var actualComment = await commentFactory.CreateFromFormAsync(new NameValueCollection())
                .ConfigureAwait(false);

            Assert.Equal(nonAnalyzedComment, actualComment);
        }
    }
}

using System.Collections.Specialized;
using System.Threading.Tasks;
using ApplicationCore.Analytics;
using Moq;
using Xunit;

namespace ApplicationCore.Tests
{
    public class CommentFactoryTests
    {
        [Fact]
        public async Task CreatesValidCommentWithOnlyRequiredValues()
        {
            var textAnalyzerMock = new Mock<ITextAnalyzer>();
            ICommentFactory commentFactory = new CommentFactory(textAnalyzerMock.Object);

            var form = new NameValueCollection
            {
                { "postId", "this-is-a-post-slug" },
                { "message", "This is the message" },
                { "name", "My Very Own Name" }
            };
            var commentResult = await commentFactory.CreateFromFormAsync(form);

            Assert.False(commentResult.HasErrors, "Comment should not have errors.");
        }
    }
}

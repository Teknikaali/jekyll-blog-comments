using System.Collections.Specialized;
using Xunit;

namespace ApplicationCore.Tests.Factories
{
    public class CommentFormFactoryTests
    {
        [Fact]
        public void CreatesCommentFormFromForm()
        {
            var factory = new CommentFormFactory();
            var commentForm = factory.CreateCommentForm(new NameValueCollection());

            Assert.NotNull(commentForm);
        }
    }
}

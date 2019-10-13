using System.Collections.Specialized;
using ApplicationCore.Model;
using Xunit;

namespace ApplicationCore.Tests.Model
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

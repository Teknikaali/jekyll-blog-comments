using ApplicationCore.Model;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace ApplicationCore.Tests.Model
{
    public class CommentFormFactoryTests
    {
        [Fact]
        public void CreatesCommentFormFromForm()
        {
            var factory = new CommentFormFactory();
            var commentForm = factory.CreateCommentForm(Mock.Of<IFormCollection>());

            Assert.NotNull(commentForm);
        }
    }
}

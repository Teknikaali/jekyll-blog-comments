using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Model
{
    public class CommentFormFactory : ICommentFormFactory
    {
        public ICommentForm CreateCommentForm(IFormCollection form)
        {
            return new CommentForm(form);
        }
    }
}

using System.Collections.Specialized;

namespace ApplicationCore.Model
{
    public class CommentFormFactory : ICommentFormFactory
    {
        public ICommentForm CreateCommentForm(NameValueCollection form)
        {
            return new CommentForm(form);
        }
    }
}

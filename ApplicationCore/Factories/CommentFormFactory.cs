using System.Collections.Specialized;

namespace ApplicationCore
{
    public class CommentFormFactory : ICommentFormFactory
    {
        public ICommentForm CreateCommentForm(NameValueCollection form)
        {
            return new CommentForm(form);
        }
    }
}

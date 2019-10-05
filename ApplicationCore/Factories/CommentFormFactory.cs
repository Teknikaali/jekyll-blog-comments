using System.Collections.Specialized;
using ApplicationCore.Model;

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

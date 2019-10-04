using System.Collections.Specialized;

namespace ApplicationCore
{
    public interface ICommentFormFactory
    {
        ICommentForm CreateCommentForm(NameValueCollection form);
    }
}

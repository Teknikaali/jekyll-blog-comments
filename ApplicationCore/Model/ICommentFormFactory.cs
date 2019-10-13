using System.Collections.Specialized;

namespace ApplicationCore.Model
{
    public interface ICommentFormFactory
    {
        ICommentForm CreateCommentForm(NameValueCollection form);
    }
}

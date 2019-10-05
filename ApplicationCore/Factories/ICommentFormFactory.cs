using System.Collections.Specialized;
using ApplicationCore.Model;

namespace ApplicationCore
{
    public interface ICommentFormFactory
    {
        ICommentForm CreateCommentForm(NameValueCollection form);
    }
}

using System.Collections.Specialized;
using System.Threading.Tasks;

namespace ApplicationCore
{
    public interface ICommentFactory
    {
        Task<CommentResult> CreateFromFormAsync(NameValueCollection form);
    }
}

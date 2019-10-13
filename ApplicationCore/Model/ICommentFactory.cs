using System.Collections.Specialized;
using System.Threading.Tasks;

namespace ApplicationCore.Model
{
    public interface ICommentFactory
    {
        Task<CommentResult> CreateFromFormAsync(NameValueCollection form);
    }
}

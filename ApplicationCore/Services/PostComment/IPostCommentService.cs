using System.Collections.Specialized;
using System.Threading.Tasks;

namespace ApplicationCore
{
    public interface IPostCommentService
    {
        Task<PostCommentResult> PostCommentAsync(NameValueCollection form);
    }
}

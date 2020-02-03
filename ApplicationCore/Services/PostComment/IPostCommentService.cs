using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore
{
    public interface IPostCommentService
    {
        Task<PostCommentResult> PostCommentAsync(IFormCollection form);
    }
}

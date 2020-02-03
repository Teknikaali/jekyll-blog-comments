using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Model
{
    public interface ICommentFactory
    {
        Task<CommentResult> CreateFromFormAsync(IFormCollection form);
    }
}

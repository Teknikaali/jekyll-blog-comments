using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Model
{
    /// <summary>
    /// Enables creating a comment from user input form.
    /// </summary>
    public interface ICommentFactory
    {
        /// <summary>
        /// Creates a <seealso cref="CommentResult"/> from user input <paramref name="form"/>.
        /// </summary>
        /// <param name="form">The comment user is posting.</param>
        /// <returns>The <seealso cref="CommentResult"/> created from <paramref name="form"/>.</returns>
        Task<CommentResult> CreateFromFormAsync(IFormCollection form);
    }
}

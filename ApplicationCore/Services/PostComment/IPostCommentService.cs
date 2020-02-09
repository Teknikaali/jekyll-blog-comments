using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore
{
    /// <summary>
    /// Enables posting comments. This is the base service for the comment posting function.
    /// </summary>
    public interface IPostCommentService
    {
        /// <summary>
        /// Post the input form as a comment to a repository
        /// </summary>
        /// <param name="form">User input for the comment as form</param>
        /// <returns>The result of posting the comment.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="form"/>Input form is null.</exception>
        Task<PostCommentResult> PostCommentAsync(IFormCollection form);
    }
}

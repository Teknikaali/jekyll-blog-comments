using System.Collections.Generic;

namespace ApplicationCore.Model
{
    /// <summary>
    /// The comment form that the user has filled.
    /// </summary>
    public interface ICommentForm
    {
        bool HasErrors { get; }
        IEnumerable<string> Errors { get; }

        /// <summary>
        /// Tries creating the comment from input form.
        /// </summary>
        /// <returns>The result of creating a comment.</returns>
        CommentResult TryCreateComment();
    }
}

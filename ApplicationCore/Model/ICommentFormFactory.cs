using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Model
{
    /// <summary>
    /// Enables creating the comment form from the user input form.
    /// </summary>
    public interface ICommentFormFactory
    {
        /// <summary>
        /// Creates the <seealso cref="ICommentForm"/> from user input <paramref name="form"/>.
        /// </summary>
        /// <param name="form">User input form (the comment).</param>
        /// <returns><seealso cref="ICommentForm"/> created from <seealso cref="IFormCollection"/>.</returns>
        ICommentForm CreateCommentForm(IFormCollection form);
    }
}

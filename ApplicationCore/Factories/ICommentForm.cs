using System.Collections.Generic;

namespace ApplicationCore
{
    public interface ICommentForm
    {
        bool IsValid { get; }
        IEnumerable<string> Errors { get; }
        CommentResult TryCreateComment();
    }
}

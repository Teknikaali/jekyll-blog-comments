using System.Collections.Generic;

namespace ApplicationCore.Model
{
    public interface ICommentForm
    {
        bool HasErrors { get; }
        IEnumerable<string> Errors { get; }
        CommentResult TryCreateComment();
    }
}

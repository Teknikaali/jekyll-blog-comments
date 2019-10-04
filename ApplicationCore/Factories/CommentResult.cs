using System.Collections.Generic;
using System.Linq;
using ApplicationCore.Model;

namespace ApplicationCore
{
    public class CommentResult
    {
        public bool HasErrors => Errors.Any();
        public Comment Comment { get; }
        public IEnumerable<string> Errors { get; }

        public CommentResult(Comment comment)
        {
            Comment = comment;
            Errors = Enumerable.Empty<string>();
        }

        public CommentResult(Comment comment, IEnumerable<string> errors) : this(comment)
        {
            Errors = errors;
        }
    }
}

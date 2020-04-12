using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Model
{
    public class CommentResult
    {
        public bool HasErrors => Errors.Any();
        
        public Comment Comment { get; }
        
        public IEnumerable<string> Errors { get; }

        public Exception? Exception { get; }

        public CommentResult(Comment comment)
        {
            Comment = comment;
            Errors = Enumerable.Empty<string>();
        }

        public CommentResult(Comment comment, IEnumerable<string> errors, Exception? exception)
            : this(comment)
        {
            Errors = errors;
            Exception = exception;
        }
    }
}

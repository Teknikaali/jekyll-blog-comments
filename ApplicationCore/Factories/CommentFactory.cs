using System.Collections.Specialized;
using System.Threading.Tasks;
using ApplicationCore.Analytics;

namespace ApplicationCore
{
    public class CommentFactory : ICommentFactory
    {
        private readonly ICommentFormFactory _commentFormFactory;
        private readonly ITextAnalyzer _textAnalyzer;

        public CommentFactory(ICommentFormFactory commentFormFactory, ITextAnalyzer textAnalyzer)
        {
            _commentFormFactory = commentFormFactory;
            _textAnalyzer = textAnalyzer;
        }

        public async Task<CommentResult> CreateFromFormAsync(NameValueCollection form)
        {
            var commentForm = _commentFormFactory.CreateCommentForm(form);
            var commentResult = commentForm.TryCreateComment();

            if (!commentForm.HasErrors)
            {
                commentResult = await _textAnalyzer.AnalyzeAsync(commentResult.Comment).ConfigureAwait(false);
            }

            return commentResult;
        }
    }
}

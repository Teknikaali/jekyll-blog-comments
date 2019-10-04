using System.Collections.Specialized;
using System.Threading.Tasks;
using ApplicationCore.Analytics;

namespace ApplicationCore
{
    public class CommentFactory : ICommentFactory
    {
        private readonly ITextAnalyzer _textAnalyzer;

        public CommentFactory(ITextAnalyzer textAnalyzer)
        {
            _textAnalyzer = textAnalyzer;
        }

        public async Task<CommentResult> CreateFromFormAsync(NameValueCollection form)
        {
            var commentForm = new CommentForm(form);
            var commentResult = commentForm.TryCreateComment();

            if (commentForm.IsValid)
            {
                if (_textAnalyzer.CanAnalyze)
                {
                    commentResult = await _textAnalyzer.AnalyzeAsync(commentResult.Comment).ConfigureAwait(false);
                }
            }

            return commentResult;
        }
    }
}

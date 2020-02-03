using System.Threading.Tasks;
using ApplicationCore.Analytics;
using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Model
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

        public async Task<CommentResult> CreateFromFormAsync(IFormCollection form)
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

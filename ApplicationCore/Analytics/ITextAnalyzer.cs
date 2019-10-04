using System.Threading.Tasks;
using ApplicationCore.Model;

namespace ApplicationCore.Analytics
{
    public interface ITextAnalyzer
    {
        bool CanAnalyze { get; }
        Task<CommentResult> AnalyzeAsync(Comment comment);
    }
}

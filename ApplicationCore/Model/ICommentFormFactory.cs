using Microsoft.AspNetCore.Http;

namespace ApplicationCore.Model
{
    public interface ICommentFormFactory
    {
        ICommentForm CreateCommentForm(IFormCollection form);
    }
}

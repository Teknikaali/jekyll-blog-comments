using System;
using System.Net;

namespace ApplicationCore
{
    public class PostCommentResult
    {
        public bool HasError => !string.IsNullOrEmpty(Error);
        public HttpStatusCode HttpStatusCode { get; }
        public Uri? RedirectUrl { get; }
        public string Error { get; }
        public Exception? Exception { get; }

        public PostCommentResult(HttpStatusCode httpStatusCode, string error)
        {
            HttpStatusCode = httpStatusCode;
            Error = error;
        }

        public PostCommentResult(HttpStatusCode httpStatusCode) : this(httpStatusCode, string.Empty)
        {

        }
        
        public PostCommentResult(HttpStatusCode httpStatusCode, string error, Exception exception) :this(httpStatusCode, error)
        {
            Exception = exception;
        }

        public PostCommentResult(HttpStatusCode httpStatusCode, Uri redirectUri) :this(httpStatusCode)
        {
            RedirectUrl = redirectUri;
        }
    }
}

using System;

namespace ApplicationCore
{
    public class PullRequestResult
    {
        public bool HasError => !string.IsNullOrEmpty(Error);

        public string Error => Exception?.Message ?? string.Empty;

        public Exception? Exception { get; }

        public PullRequestResult()
        {

        }

        public PullRequestResult(Exception e)
        {
            Exception = e;
        }
    }
}

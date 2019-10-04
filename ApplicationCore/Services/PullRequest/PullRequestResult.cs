namespace ApplicationCore
{
    public class PullRequestResult
    {
        public bool HasError => !string.IsNullOrEmpty(Error);
        public string Error { get; }

        public PullRequestResult()
        {
            Error = string.Empty;
        }

        public PullRequestResult(string error)
        {
            Error = error;
        }
    }
}

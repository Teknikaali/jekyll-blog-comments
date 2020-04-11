namespace ApplicationCore
{
    public class WebConfiguration
    {
        /// <summary>
        /// Required
        /// </summary>
        public string GitHubToken { get; set; } = "";

        /// <summary>
        /// Required
        /// </summary>
        public string PullRequestRepository { get; set; } = "";

        /// <summary>
        /// Required
        /// </summary>
        public string Website { get; set; } = "";

        /// <summary>
        /// Optional
        /// </summary>
        public string FallbackCommitEmail { get; set; } = "";

        /// <summary>
        /// Optional
        /// </summary>
        public string TextAnalyticsSubscriptionKey { get; set; } = "";

        /// <summary>
        /// Optional
        /// </summary>
        public string TextAnalyticsRegion { get; set; } = "";

        /// <summary>
        /// Optional
        /// </summary>
        public string TextAnalyticsLanguage { get; set; } = "";
    }
}
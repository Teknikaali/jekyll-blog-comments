using Microsoft.Extensions.Configuration;

namespace JekyllBlogCommentsAzure
{
    public class WebConfigurator
    {
        private static IConfigurationRoot _config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        public string CommentWebsiteUrl => _config["CommentWebsiteUrl"];

        public string GitHubToken => _config["GitHubToken"];

        public string PullRequestRepository => _config["PullRequestRepository"];

        public string CommentFallbackCommitEmail => _config["CommentFallbackCommitEmail"];

        public string SentimentAnalysisSubscriptionKey => _config["SentimentAnalysis.SubscriptionKey"];

        public string SentimentAnalysisRegion => _config["SentimentAnalysis.Region"];

        public string SentimentAnalysisLang => _config["SentimentAnalysis.Lang"];
    }
}

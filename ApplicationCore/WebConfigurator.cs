using System;
using Microsoft.Extensions.Configuration;

namespace ApplicationCore
{
    public interface IWebConfigurator
    {
        Uri CommentWebsiteUrl { get; }
        string GitHubToken { get; }
        string PullRequestRepository { get; }
        string CommentFallbackCommitEmail { get; }
        string TextAnalyticsSubscriptionKey { get; }
        string TextAnalyticsRegion { get; }
        string TextAnalyticsLang { get; }

        bool PushToGitHub { get; }
    }

    public class WebConfigurator : IWebConfigurator
    {
        private static readonly IConfigurationRoot _config = 
            new ConfigurationBuilder().AddEnvironmentVariables().Build();

        public static bool IsDevelopmentMode => 
            _config.GetValue("AzureWebJobsStorage", defaultValue: "false") != "false"
            ? _config["AzureWebJobsStorage"] == "UseDevelopmentStorage=true"
            : false;

        public Uri CommentWebsiteUrl => new Uri(_config["CommentWebsiteUrl"], UriKind.Absolute);

        public string GitHubToken => _config["GitHubToken"];

        public string PullRequestRepository => _config["PullRequestRepository"];

        public string CommentFallbackCommitEmail => _config["CommentFallbackCommitEmail"];

        public string TextAnalyticsSubscriptionKey => _config["TextAnalytics.SubscriptionKey"];

        public string TextAnalyticsRegion => _config["TextAnalytics.Region"];

        public string TextAnalyticsLang => _config["TextAnalytics.Lang"];

        public bool PushToGitHub => _config.GetValue("PushToGitHub", defaultValue: false);
    }
}

using System.ComponentModel.DataAnnotations;

namespace ApplicationCore
{
    public class WebConfiguration
    {
        [Required]
        public string GitHubToken { get; set; } = "";

        [Required]
        [RegularExpression(
            @"^[a-zA-Z0-9]+\/{1}[a-zA-Z0-9]+$",
            ErrorMessage = "PullRequestRepository: invalid format. Valid format is \"yourAlias/YourRepository\".")]
        public string PullRequestRepository { get; set; } = "";

        [Required]
        public string Website { get; set; } = "";

        public string FallbackCommitEmail { get; set; } = "";

        public string TextAnalyticsSubscriptionKey { get; set; } = "";

        public string TextAnalyticsRegion { get; set; } = "";

        public string TextAnalyticsLanguage { get; set; } = "";
    }
}
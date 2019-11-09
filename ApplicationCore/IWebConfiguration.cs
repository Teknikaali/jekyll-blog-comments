namespace ApplicationCore
{
    public interface IWebConfiguration
    {
        CommentConfig Comment { get; }
        GitHubConfig GitHub { get; }
        TextAnalyticsConfig TextAnalytics { get; }
    }
}

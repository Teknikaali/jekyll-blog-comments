using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace ApplicationCore
{
    public interface IWebConfiguration
    {
        CommentConfig Comment { get; }
        GitHubConfig GitHub { get; }
        TextAnalyticsConfig TextAnalytics { get; }
    }

    public class WebConfiguration : IWebConfiguration
    {
        public CommentConfig Comment => _configProvider.GetConfig<CommentConfig>(_config);

        public GitHubConfig GitHub => _configProvider.GetConfig<GitHubConfig>(_config);

        public TextAnalyticsConfig TextAnalytics => _configProvider.GetConfig<TextAnalyticsConfig>(_config);

        private readonly IConfigurationRoot _config;
        private readonly IConfigProvider _configProvider;

        public WebConfiguration(IConfigurationRoot config, IConfigProvider provider)
        {
            _config = config;
            _configProvider = provider;
        }
    }

    public class CommentConfig
    {
        public Uri WebsiteUrl { get; set; } = null!;

        public MailAddress FallbackCommitEmailAddress => new MailAddress(FallbackCommitEmail);

        /// <summary>
        /// <see cref="MailAddress"/> is non-serializable. This property is used as a proxy when
        /// <see cref="BinderOptions.BindNonPublicProperties"/> is set to <c>true</c> and when binding the section to
        /// the actual config instance.
        /// </summary>
        private string FallbackCommitEmail { get; set; } = "";

        public CommentConfig() { }

        public CommentConfig(string website, string fallbackCommitEmailAddress)
        {
            WebsiteUrl = GetValidUri(website);
            FallbackCommitEmail = GetValidEmail(fallbackCommitEmailAddress);
        }

        private Uri GetValidUri(string website)
        {
            if (!Uri.TryCreate(website, UriKind.Absolute, out var uri)
                && (uri == null || (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        CommentResources.WebsiteInvalidUriErrorMessage,
                        website),
                    nameof(website));
            }

            return uri;
        }

        private string GetValidEmail(string email)
        {
            if(!new EmailAddressAttribute().IsValid(email))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        CommentResources.FallbackCommitEmailInvalidErrorMessage,
                        email),
                    nameof(email));
            }
            
            return email;
        }
    }

    public class GitHubConfig
    {
        public string Token { get; set; } = "";
        public string PullRequestRepository { get; set; } = "";
    }

    public class TextAnalyticsConfig
    {
        public string SubscriptionKey { get; set; } = "";
        public string Region { get; set; } = "";
        public string Language { get; set; } = "";
    }
}
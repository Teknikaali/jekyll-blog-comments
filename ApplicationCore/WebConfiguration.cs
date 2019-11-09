using System;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ApplicationCore
{
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

    public interface IConfigProvider
    {
        T GetConfig<T>(IConfigurationRoot config) where T : new();
    }

    public class ConfigProvider : IConfigProvider
    {
        public T GetConfig<T>(IConfigurationRoot configRoot) where T : new()
        {
            if (configRoot is null)
            {
                throw new ArgumentNullException(nameof(configRoot));
            }
            
            var config = new T();
            configRoot.GetSection(typeof(T).Name).Bind(config, x => x.BindNonPublicProperties = true);
            
            return config;
        }
    }

    public class CommentConfig
    {
        public Uri WebsiteUrl { get; set; } = new Uri("", UriKind.Relative);

        public MailAddress FallbackCommitEmailAddress
        {
            get => new MailAddress(FallbackCommitEmail);
            set => FallbackCommitEmail = value?.ToString() ?? "";
        }

        /// <summary>
        /// <see cref="MailAddress"/> is non-serializable. This property is used as a proxy when
        /// <see cref="BinderOptions.BindNonPublicProperties"/> is set to <c>true</c> and when binding the section to
        /// the actual config instance.
        /// </summary>
        private string FallbackCommitEmail { get; set; } = "foo@example.com";
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
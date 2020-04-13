using System;
using System.Linq;
using ApplicationCore;
using ApplicationCore.Analytics;
using ApplicationCore.Model;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JekyllBlogCommentsAzure
{
    /// <summary>
    /// Enables a simple way to add the <seealso cref="PostCommentService"/> to a <seealso cref="ServiceCollection"/>.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures the required <see cref="IPostCommentService"/> to enable the function to post comments.
        /// </summary>
        /// <param name="services">Container for a collection of service providers</param>
        /// <returns><paramref name="services"/> collection</returns>
        public static IServiceCollection AddPostCommentService(this IServiceCollection services)
        {
            var executionContextOptions = services.BuildServiceProvider()
                    .GetService<IOptions<ExecutionContextOptions>>().Value;
            var currentDirectory = executionContextOptions.AppDirectory;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            services.AddOptions<WebConfiguration>();
            services.ConfigureAndValidate<WebConfiguration>(configuration);

            services.AddSingleton<ICommentFormFactory, CommentFormFactory>();
            services.AddSingleton<ICredentialsFactory, CredentialsFactory>();
            services.AddSingleton<ITextAnalyticsClientFactory, TextAnalyticsClientFactory>();
            services.AddSingleton<ITextAnalyzer, TextAnalyzer>();
            services.AddSingleton<ICommentFactory, CommentFactory>();
            services.AddSingleton<ISerializerFactory, SerializerFactory>();
            services.AddSingleton<IGitHubClientFactory, GitHubClientFactory>();
            services.AddSingleton<IPullRequestService, PullRequestService>();

            services.AddSingleton<IPostCommentService, PostCommentService>();

            return services;
        }

        public static IServiceCollection ConfigureAndValidate<T>(
            this IServiceCollection services,
            IConfiguration config) where T : class
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            return services
            .Configure<T>(config.GetSection("Values"))
            .PostConfigure<T>(settings =>
            {
                var configErrors = settings.ValidationErrors().ToList();
                if (configErrors.Any())
                {
                    var errors = string.Join(",", configErrors);
                    var count = configErrors.Count;
                    var configType = typeof(T).Name;
                    throw new ApplicationException(
                        $"Found {count} configuration error(s) in {configType}: {errors}");
                }
            });
        }
    }
}

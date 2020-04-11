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

            var configRoot = new ConfigurationBuilder()
                .SetBasePath(currentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var config = new WebConfiguration();
            configRoot.GetSection("Values").Bind(config);

            var commentFactory = new CommentFactory(
                new CommentFormFactory(),
                new TextAnalyzer(config, new TextAnalyticsClientFactory(new CredentialsFactory())));
            var pullRequestService = new PullRequestService(
                    config,
                    new SerializerFactory(),
                    new GitHubClientFactory(config));

            var postCommentService = new PostCommentService(config, commentFactory, pullRequestService);
            services.AddSingleton<IPostCommentService>(postCommentService);

            return services;
        }
    }
}

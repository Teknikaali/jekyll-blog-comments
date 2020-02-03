using ApplicationCore;
using ApplicationCore.Analytics;
using ApplicationCore.Model;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace JekyllBlogCommentsAzure
{
    public static class IServiceCollectionExtensions
    {
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

            var config = new WebConfiguration(configRoot, new ConfigProvider());

            var commentFactory = new CommentFactory(
                new CommentFormFactory(),
                new TextAnalyzer(config.TextAnalytics, new TextAnalyticsClientFactory(new CredentialsFactory())));
            var pullRequestService = new PullRequestService(
                    config.GitHub,
                    config.Comment,
                    new SerializerFactory().BuildSerializer(),
                    new GitHubClientFactory(config.GitHub).CreateClient());

            var postCommentService = new PostCommentService(config.Comment, commentFactory, pullRequestService);
            services.AddSingleton<IPostCommentService>(postCommentService);

            return services;
        }
    }
}

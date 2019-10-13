using System;
using ApplicationCore;
using ApplicationCore.Analytics;
using ApplicationCore.Model;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// To register the Configure method we need to specify the type name used during startup
[assembly: FunctionsStartup(typeof(JekyllBlogCommentsAzure.Startup))]

namespace JekyllBlogCommentsAzure
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddSingleton(CreatePostCommentService);

            static IPostCommentService CreatePostCommentService(IServiceProvider serviceProvider)
            {
                var config = new WebConfigurator();
                var commentFactory = new CommentFactory(
                    new CommentFormFactory(),
                    new TextAnalyzer(config, new TextAnalyticsClientFactory(new CredentialsFactory())));
                var pullRequestService = config.PushToGitHub
                    ? new PullRequestService(
                        config,
                        new SerializerFactory().BuildSerializer(),
                        new GitHubClientFactory(config).CreateClient()) as IPullRequestService
                    : new NoOpPullRequestService(serviceProvider.GetService<ILoggerProvider>());

                return new PostCommentService(config, commentFactory, pullRequestService);
            }
        }
    }
}

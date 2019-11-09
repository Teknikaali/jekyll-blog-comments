using System;
using ApplicationCore;
using ApplicationCore.Analytics;
using ApplicationCore.Model;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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

            IPostCommentService CreatePostCommentService(IServiceProvider serviceProvider)
            {
                var executionContextOptions = builder.Services.BuildServiceProvider()
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

                return new PostCommentService(config.Comment, commentFactory, pullRequestService);
            }
        }
    }
}

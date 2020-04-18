using System;
using System.IO;
using ApplicationCore;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace JekyllBlogCommentsAzure.Tests
{
    public class IServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddPostCommentServiceInitializesCorrectly()
        {
            var servicesMock = new Mock<ServiceCollection> { CallBase = true };
            servicesMock.Object.AddOptions<ExecutionContextOptions>();
            servicesMock.Object.PostConfigure<ExecutionContextOptions>(
                x => x.AppDirectory = Directory.GetCurrentDirectory());

            // Manually define the configuration because Azure doesn't have a "Values" collection
            // like the local.settings.json has
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            servicesMock.Object.AddOptions<WebConfiguration>();
            servicesMock.Object.Configure<WebConfiguration>(configuration.GetSection("Values"));

            IServiceCollectionExtensions.AddPostCommentService(servicesMock.Object);

            Assert.NotNull(servicesMock.Object.BuildServiceProvider().GetService<IPostCommentService>());
        }
    }
}

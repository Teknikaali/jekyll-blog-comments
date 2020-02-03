using System.IO;
using ApplicationCore;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace JekyllBlogCommentsAzure.Tests
{
    public class IServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddPostCommentServiceInitializesCorrectly()
        {
            var services = new ServiceCollection();
            services.AddOptions<ExecutionContextOptions>();
            services.PostConfigure<ExecutionContextOptions>(x => x.AppDirectory = Directory.GetCurrentDirectory());
            
            IServiceCollectionExtensions.AddPostCommentService(services);

            Assert.NotNull(services.BuildServiceProvider().GetService<IPostCommentService>());
        }
    }
}

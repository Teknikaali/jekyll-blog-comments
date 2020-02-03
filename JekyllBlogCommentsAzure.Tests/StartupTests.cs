using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ApplicationCore;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace JekyllBlogCommentsAzure.Tests
{
    public class StartupTests
    {
        [Fact]
        public void ConfigureAddsPostCommentService()
        {
            var builderMock = new Mock<IFunctionsHostBuilder>();
            var services = new ServiceCollection();
            builderMock.Setup(x => x.Services).Returns(services);
            services.AddOptions<ExecutionContextOptions>();
            services.PostConfigure<ExecutionContextOptions>(x => x.AppDirectory = Directory.GetCurrentDirectory());

            new Startup().Configure(builderMock.Object);

            var serviceDescriptor = services
                .Cast<ServiceDescriptor>()
                .First(x => x.ServiceType == typeof(IPostCommentService));

            Assert.Equal(typeof(IPostCommentService), serviceDescriptor.ServiceType);
            Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
        }

        [Fact]
        public void ThrowIfConfigureWithoutBuilder()
        {
            Assert.Throws<ArgumentNullException>(() => new Startup().Configure((IFunctionsHostBuilder)null!));
        }
    }
}

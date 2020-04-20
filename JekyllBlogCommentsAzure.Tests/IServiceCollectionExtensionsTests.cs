using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using ApplicationCore;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace JekyllBlogCommentsAzure.Tests
{
    public class IServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddPostCommentServiceInitializesCorrectly()
        {
            SetupLocalEnvironmentVariables();

            var services = new ServiceCollection();
            services.AddOptions<ExecutionContextOptions>();
            services.PostConfigure<ExecutionContextOptions>(
                x => x.AppDirectory = Directory.GetCurrentDirectory());

            IServiceCollectionExtensions.AddPostCommentService(services);

            Assert.NotNull(services.BuildServiceProvider().GetService<IPostCommentService>());
        }

        private static void SetupLocalEnvironmentVariables()
        {
            // Environment variables aren't automatically set from local.settings.json file
            // so we must manually map them here before running dependant tests
            var basePath = Directory.GetCurrentDirectory();
            var settings = JsonConvert.DeserializeObject<LocalSettings>(
                File.ReadAllText(basePath + "\\local.settings.json"));

            foreach (var setting in settings.Values)
            {
                Environment.SetEnvironmentVariable(setting.Key, setting.Value);
            }
        }

        /// <summary>
        /// Strongly typed local.settings.json
        /// </summary>
        [SuppressMessage(
            "Performance",
            "CA1812:Avoid uninstantiated internal classes",
            Justification = "The class is created through late-bound reflection methods")]
        private class LocalSettings
        {
            public bool IsEncrypted { get; set; }
            public Dictionary<string, string> Values { get; set; } = new Dictionary<string, string>();
        }
    }
}

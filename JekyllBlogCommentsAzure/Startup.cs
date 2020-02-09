using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

// To register the Configure method we need to specify the type name used during startup
[assembly: FunctionsStartup(typeof(JekyllBlogCommentsAzure.Startup))]

namespace JekyllBlogCommentsAzure
{
    /// <summary>
    /// Defines startup configuration action that is performed as part of the Functions runtime startup.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <summary>
        /// Performs the startup configuration action.
        /// </summary>
        /// <param name="builder">The IWebJobsBuilder that can be used to configure the host.</param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services.AddPostCommentService();
        }
    }
}

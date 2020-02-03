using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

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

            builder.Services.AddPostCommentService();
        }
    }
}

using System;
using Microsoft.Extensions.Configuration;

namespace ApplicationCore
{
    public interface IConfigProvider
    {
        T GetConfig<T>(IConfigurationRoot config) where T : new();
    }

    public class ConfigProvider : IConfigProvider
    {
        public T GetConfig<T>(IConfigurationRoot configRoot) where T : new()
        {
            if (configRoot is null)
            {
                throw new ArgumentNullException(nameof(configRoot));
            }

            var config = new T();
            configRoot.GetSection(typeof(T).Name).Bind(config, x => x.BindNonPublicProperties = true);

            return config;
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Moq;
using Xunit;

namespace ApplicationCore.Tests.Configuration
{
    public class ConfigProviderTests
    {
        [Fact]
        public void GetConfigThrowsIfConfigRootIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ConfigProvider().GetConfig<object>(null!));
        }

        [Fact]
        public void GetConfigReturnsConfigInstance()
        {
            var valueFromConfig = "ValueFromConfig";
            var configRoot = new ConfigurationRoot(new List<IConfigurationProvider> {
                new MemoryConfigurationProvider(
                    new MemoryConfigurationSource
                    {
                        InitialData = new KeyValuePair<string, string>[]
                        {
                            new KeyValuePair<string, string>("TestConfig:Value", valueFromConfig)
                        }
                    })});

            var config = new ConfigProvider().GetConfig<TestConfig>(configRoot);

            Assert.Equal(valueFromConfig, config.Value);
        }

        internal class TestConfig
        {
            public string Value { get; set; } = "DefaultValue";
        }
    }
}

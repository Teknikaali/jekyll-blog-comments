﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ApplicationCore.Configuration.Tests
{
    public class WebConfiguratorTests
    {
        private const string _websiteUrl = "http://www.example.com/";
        private const string _fallbackCommitEmail = "redacted@example.com";
        private const string _gitHubTestToken = "GitHubTestToken";
        private const string _pullRequestRepository = "http://www.example.com/";
        private const string _language = "en";
        private const string _region = "Region";
        private const string _subscriptionKey = "SubscriptionKey";

        [Theory]
        [MemberData(nameof(TestCases))]
        public void ReturnsConfigurationValue(ConfigurationTestCase testCase)
        {
            var commentConfig = new CommentConfig(_websiteUrl, _fallbackCommitEmail);
            var gitHubConfig = new GitHubConfig
            {
                PullRequestRepository = _pullRequestRepository,
                Token = _gitHubTestToken
            };
            var textAnalyticsConfig = new TextAnalyticsConfig
            {
                Language = _language,
                Region = _region,
                SubscriptionKey = _subscriptionKey
            };
            
            var configRootMock = new Mock<IConfigurationRoot>();

            var configProviderMock = new Mock<IConfigProvider>();
            configProviderMock.Setup(x => x.GetConfig<CommentConfig>(It.IsAny<IConfigurationRoot>()))
                .Returns(commentConfig);
            configProviderMock.Setup(x => x.GetConfig<GitHubConfig>(It.IsAny<IConfigurationRoot>()))
                .Returns(gitHubConfig);
            configProviderMock.Setup(x => x.GetConfig<TextAnalyticsConfig>(It.IsAny<IConfigurationRoot>()))
                .Returns(textAnalyticsConfig);
            var config = new WebConfiguration(configRootMock.Object, configProviderMock.Object);

            var rootProperty = typeof(IWebConfiguration).GetProperty(testCase.RootName)
                ?? throw new NullReferenceException($"Root \"{testCase.RootName}\" couldn't be found.");
            var root = rootProperty.GetValue(config)
                ?? throw new NullReferenceException($"Couldn't get value of \"{rootProperty.Name}\".");
            
            var resultProperty = root.GetType().GetProperty(testCase.PropertyName)
                ?? throw new NullReferenceException($"Property \"{testCase.PropertyName}\" couldn't be found.");
            var result = resultProperty.GetValue(root);

            Assert.Equal(testCase.ObjectValue, result);
        }

        [Fact]
        public void InvalidProvidedValueThrows()
        {
            Assert.Throws<ArgumentException>(() => new CommentConfig(_websiteUrl, ""));
            Assert.Throws<ArgumentException>(() => new CommentConfig("", _fallbackCommitEmail));
            Assert.Throws<ArgumentException>(() => new CommentConfig("WrongFormat", _fallbackCommitEmail));
        }

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[]
            {
                new ConfigurationTestCase(
                    x => x.Comment.WebsiteUrl,
                    _websiteUrl,
                    x => new Uri(x, UriKind.Absolute))
            };
            yield return new object[]
            {
                new ConfigurationTestCase(
                    x => x.Comment.FallbackCommitEmailAddress,
                    _fallbackCommitEmail,
                    x => new MailAddress(x))
            };
            yield return new object[]
            {
                new ConfigurationTestCase(
                    x => x.GitHub.Token,
                    _gitHubTestToken)
            };
            yield return new object[]
            {
                new ConfigurationTestCase(
                    x => x.GitHub.PullRequestRepository,
                    _pullRequestRepository)
            };
            yield return new object[]
            {
                new ConfigurationTestCase(
                    x => x.TextAnalytics.Language,
                    _language)
            };
            yield return new object[]
            {
                new ConfigurationTestCase(
                    x => x.TextAnalytics.Region,
                    _region)
            };
            yield return new object[]
            {
                new ConfigurationTestCase(
                    x => x.TextAnalytics.SubscriptionKey,
                    _subscriptionKey)
            };
        }
    }

    public class ConfigurationTestCase
    {
        public string RootName { get; set; } = null!;
        public string PropertyName { get; set; } = null!;
        public string PropertyValue { get; set; } = null!;
        public object ObjectValue { get; set; } = null!;

        public ConfigurationTestCase(Expression<Func<IWebConfiguration, object>> getPropertyNameExpression, string propertyValue)
            : this(getPropertyNameExpression, propertyValue, x => x) { }

        public ConfigurationTestCase(Expression<Func<IWebConfiguration, object>> getPropertyNameExpression, string propertyValue, Func<string, object> convert)
        {
            var propertyPath = getPropertyNameExpression.Body.ToString().Split('.').Skip(1);
            RootName = propertyPath.First();
            PropertyName = propertyPath.Last();
            PropertyValue = propertyValue;
            ObjectValue = convert(PropertyValue);
        }
    }
}

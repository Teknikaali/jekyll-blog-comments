using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Model;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;

namespace ApplicationCore.Analytics
{
    public class TextAnalyzer : ITextAnalyzer
    {
        private readonly string _subscriptionKey;
        private readonly string _region;
        private readonly string _lang;

        private readonly IWebConfigurator _config;
        private readonly ITextAnalyticsClientFactory _textAnalyticsClientFactory;

        public bool CanAnalyze => !string.IsNullOrEmpty(_subscriptionKey);

        public TextAnalyzer(IWebConfigurator config, ITextAnalyticsClientFactory textAnalyticsClientFactory)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _textAnalyticsClientFactory = textAnalyticsClientFactory
                ?? throw new ArgumentNullException(nameof(textAnalyticsClientFactory));

            _subscriptionKey = _config.TextAnalyticsSubscriptionKey;
            _region = config.TextAnalyticsRegion;
            _lang = config.TextAnalyticsLang;
        }

        public async Task<CommentResult> AnalyzeAsync(Comment comment)
        {
            if (comment is null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            Comment analyzedComment;

            using (var client = _textAnalyticsClientFactory.CreateClient(_subscriptionKey, _region))
            {
                var result = await client.SentimentBatchAsync(
                    new MultiLanguageBatchInput(
                        SplitFiveHundredChars(comment.Message).ToList()))
                    .ConfigureAwait(false);

                analyzedComment = comment.WithScore($"{result.Documents[0].Score:0.00}");
            }

            return new CommentResult(analyzedComment);
        }

        private IEnumerable<MultiLanguageInput> SplitFiveHundredChars(string input)
        {
            var id = 0;
            for (var i = 0; i < input.Length; i += 500)
            {
                yield return new MultiLanguageInput(
                    _lang,
                    id.ToString(CultureInfo.InvariantCulture),
                    input.Substring(i, Math.Min(500, input.Length - i)));
                id++;
            }
        }
    }
}

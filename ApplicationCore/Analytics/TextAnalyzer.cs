using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Model;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Extensions.Options;

namespace ApplicationCore.Analytics
{
    public class TextAnalyzer : ITextAnalyzer
    {
        private const int _splitTextLength = 500;

        private readonly WebConfiguration _config;
        private readonly ITextAnalyticsClientFactory _textAnalyticsClientFactory;

        public bool CanAnalyze => !string.IsNullOrEmpty(_config.TextAnalyticsSubscriptionKey);

        public TextAnalyzer(IOptions<WebConfiguration> config, ITextAnalyticsClientFactory textAnalyticsClientFactory)
        {
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _textAnalyticsClientFactory = textAnalyticsClientFactory
                ?? throw new ArgumentNullException(nameof(textAnalyticsClientFactory));
        }

        public async Task<CommentResult> AnalyzeAsync(Comment comment)
        {
            if (comment is null)
            {
                throw new ArgumentNullException(nameof(comment));
            }
            if (!CanAnalyze)
            {
                return new CommentResult(comment);
            }

            Comment analyzedComment;

            using (var client = _textAnalyticsClientFactory.CreateClient(_config.TextAnalyticsSubscriptionKey, _config.TextAnalyticsRegion))
            {
                var result = await client.SentimentBatchAsync(
                    new MultiLanguageBatchInput(SplitFiveHundredChars(comment.Message).ToList()))
                    .ConfigureAwait(false);

                var score = result.Documents?.Any() == true ? result.Documents[0].Score : 0;
                analyzedComment = comment.WithScore($"{score:0.00}");
            }

            return new CommentResult(analyzedComment);
        }

        private IEnumerable<MultiLanguageInput> SplitFiveHundredChars(string input)
        {
            var inputs = new List<MultiLanguageInput>();

            if (string.IsNullOrEmpty(input))
            {
                return inputs;
            }

            var id = 0;
            for (var i = 0; i < input.Length; i += _splitTextLength)
            {
                var multiLanguageInput = new MultiLanguageInput
                {
                    Id = id.ToString(CultureInfo.InvariantCulture),
                    Language = _config.TextAnalyticsLanguage
                };

                if((i + _splitTextLength) < input.Length)
                {
                    multiLanguageInput.Text = input.Substring(i, Math.Min(500, input.Length - i));
                    id++;
                }
                else
                {
                    multiLanguageInput.Text = input;
                }

                inputs.Add(multiLanguageInput);
            }

            return inputs;
        }
    }
}

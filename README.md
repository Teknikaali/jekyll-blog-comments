# Jekyll Blog Comments Azure Function

An Azure Function App that receives comment form posts and creates a pull request against your GitHub repository as part of the [jekyll-blog-comments](https://github.com/damieng/jekyll-blog-comments) system.

The app includes just one function:

* `PostComment` - receives form POST submission and creates a PR to add the comment to your Jekyll site

# Customizations

This repository is a heavily modified version of the original:
* Core functionality has been moved to a different project file making the PostComment just a thin passthrough layer
* Heavily uses Dependency Injection to make code more testable
* Adds xUnit unit test and code coverage reporting with Coverlet + ReportGenerator
* Automatically deploy to Azure

## Code coverage status

[![Branch coverage](https://github.com/Teknikaali/jekyll-blog-comments/blob/gh-pages/badge_branchcoverage.svg?raw=true&sanitize=true)](https://teknikaali.github.io/jekyll-blog-comments/) [![Line coverage](https://github.com/Teknikaali/jekyll-blog-comments/blob/gh-pages/badge_linecoverage.svg?raw=true&sanitize=true)](https://teknikaali.github.io/jekyll-blog-comments/)

Coverage reports are located in the [gh-pages branch](https://github.com/Teknikaali/jekyll-blog-comments/tree/gh-pages). Click the badge to see the full coverage report.

## Setup

To set this up, you'll need to have an [Azure Portal account](https://portal.azure.com).

1. Fork this repository
2. [Create a **v3** Azure Function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-azure-function)
3. [Create subscription key in Microsoft Azure Recognition](https://docs.microsoft.com/en-us/azure/cognitive-services/cognitive-services-apis-create-account) **optional**
4. [Set up your function to deploy from your fork](https://docs.microsoft.com/en-us/azure/azure-functions/scripts/functions-cli-create-function-app-github-continuous)
5. Set up the following [App Settings for your Azure Function](https://docs.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings)

| Setting | Value | Required
| -------- | ------- | ---
| `PullRequestRepository` | `owner/name` of the repository that houses your Jekyll site for pull requests to be created against. For example, `haacked/haacked.com` will post to https://github.com/haacked/haacked.com | *
| `GitHubToken` | A [GitHub personal access token](https://help.github.com/articles/creating-a-personal-access-token-for-the-command-line/) with access to edit your target repository. | *
| `Website` | The URL to the website that hosts the comments. This is used to make sure the correct site is posting comments to the receiver. | *
| `FallbackCommitEmail` | The email address to use for GitHub commits and PR's if the form does not supply one.
| `TextAnalyticsSubscriptionKey` | Subscription Key for Microsoft Azure Recognition, if you don't want to use, just leave empty.
| `TextAnalyticsRegion` | Region for your Subscription key (E.g.: westus)
| `TextAnalyticsLanguage` | Language for comment, [find lang code here](https://docs.microsoft.com/en-us/azure/cognitive-services/text-analytics/language-support)


## Local development

* You'll need add `local.settings.json` to `JekyllBlogCommentsAzure` and `JekyllBlogCommentsAzure.Tests` projects as these configuration files are excluded from Git with .gitignore.
** Example of `local.settings.json`:
```
{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "Website": "https://example.com",
    "FallbackCommitEmail": "redacted@example.com",
    "GitHubToken": "TestToken",
    "PullRequestRepository": "YourAlias/yourRepository"
  }
}
```
*Note*: with this example configuration you can't commit/push because of the invalid GitHub token.
* Run/Debug `JekyllBlogCommentsAzure` project
* You can test in local environment by using Postman (or similar REST client)
** Local Postman environment and an example collection has been provided inside `Postman`-folder
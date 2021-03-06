﻿using System;
using System.Threading.Tasks;
using ApplicationCore.Model;
using Microsoft.Extensions.Options;
using Octokit;
using YamlDotNet.Serialization;

namespace ApplicationCore
{
    public class PullRequestService : IPullRequestService
    {
        private readonly WebConfiguration _config;
        private readonly ISerializer _serializer;
        private readonly IGitHubClient _github;

        public PullRequestService(
            IOptions<WebConfiguration> config,
            ISerializerFactory serializerFactory,
            IGitHubClientFactory githubFactory)
        {
            if (serializerFactory is null)
            {
                throw new ArgumentNullException(nameof(serializerFactory));
            }
            if (githubFactory is null)
            {
                throw new ArgumentNullException(nameof(githubFactory));
            }

            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _serializer = serializerFactory.BuildSerializer();
            _github = githubFactory.CreateClient();
        }

        public async Task<PullRequestResult> TryCreatePullRequestAsync(Comment comment)
        {
            if (comment is null)
            {
                throw new ArgumentNullException(nameof(comment));
            }

            // Get a reference to our GitHub repository
            var repoOwnerName = _config.PullRequestRepository.Split('/');
            Repository repository;

            try
            {
                repository = await _github.Repository.Get(repoOwnerName[0], repoOwnerName[1])
                    .ConfigureAwait(false);
            }
            catch (IndexOutOfRangeException e)
            {
                return new PullRequestResult(e);
            }
            catch (ApiException e)
            {
                return new PullRequestResult(e);
            }

            // Create a new branch from the default branch
            var defaultBranch = await _github.Repository.Branch.Get(repository.Id, repository.DefaultBranch)
                .ConfigureAwait(false);
            var newBranch = await _github.Git.Reference.Create(
                repository.Id,
                new NewReference($"refs/heads/comment-{comment.Id}", defaultBranch.Commit.Sha))
                .ConfigureAwait(false);

            // Create a new file with the comments in it
            var fileRequest = new CreateFileRequest(
                $"Comment by {comment.Name} on {comment.PostId}",
                _serializer.Serialize(comment),
                newBranch.Ref) // TODO: NEWBRANCH REF NULL
            {
                Committer = new Committer(
                    comment.Name,
                    comment.Email ?? _config.FallbackCommitEmail,
                    comment.Date)
            };
            await _github.Repository.Content.CreateFile(
                repository.Id,
                $"_data/comments/{comment.PostId}/{comment.Id}.yml",
                fileRequest)
                .ConfigureAwait(false);

            // Create a pull request for the new branch and file
            await _github.Repository.PullRequest.Create(
                repository.Id,
                new NewPullRequest(fileRequest.Message, newBranch.Ref, defaultBranch.Name)
                {
                    Body = $"avatar: <img src=\"{comment.Avatar}\" width=\"64\" height=\"64\" />" +
                    $"\n\nScore: {comment.Score}" +
                    $"\n\n{comment.Message}"
                }).ConfigureAwait(false);

            return new PullRequestResult();
        }
    }
}

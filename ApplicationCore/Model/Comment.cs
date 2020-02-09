using System;
using System.Globalization;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace ApplicationCore.Model
{
    /// <summary>
    /// Represents a Comment to be written to the repository in YML format.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Valid characters when mapping from the blog post slug to a file path
        /// </summary>
        static readonly Regex _validPathChars = new Regex(@"[^a-zA-Z0-9-]"); 

        [YamlIgnore]
        public string PostId { get; }

        public string Id { get; }

        /// <summary>
        /// The comment's creation date
        /// </summary>
        public DateTime Date { get; }

        public string Name { get; }

        public string Message { get; }

        public string? Email { get; }

        public string Score { get; }

        /// <summary>
        /// Optional Uri to the image/resource to be used as the user's profile avatar
        /// </summary>
        [YamlMember(SerializeAs = typeof(string))]
        public Uri? Avatar { get; }

        /// <summary>
        /// Optional Uri for the user's website
        /// </summary>
        [YamlMember(SerializeAs = typeof(string))]
        public Uri? Url { get; }

        public Comment(
            string postId,
            string message,
            string name,
            string? email = null,
            Uri? url = null,
            string? avatar = null,
            string? score = null)
        {
            PostId = _validPathChars.Replace(postId, "-");

            Message = message;
            Name = name;
            Email = email;
            Url = url;

            Date = DateTime.UtcNow;
            Id = new { PostId, Name, Message, Date }.GetHashCode().ToString("x8", CultureInfo.InvariantCulture);

            if (Uri.TryCreate(avatar, UriKind.Absolute, out var avatarUrl))
                Avatar = avatarUrl;

            Score = score ?? "Not configured";
        }
        
        /// <summary>
        /// Copy constructor for internal use only
        /// </summary>
        private Comment(
            string postId,
            string message,
            string name,
            string? email,
            Uri? url,
            DateTime date,
            string id,
            Uri? avatar,
            string? score)
        {
            PostId = postId;
            Message = message;
            Name = name;
            Email = email;
            Url = url;
            Date = date;
            Id = id;
            Avatar = avatar;
            Score = score ?? "Not configured";
        }

        public Comment WithScore(string score)
        {
            return new Comment(PostId, Message, Name, Email, Url, Date, Id, Avatar, score);
        }
    }
}

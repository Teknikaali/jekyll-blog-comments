using ApplicationCore.Model;
using Xunit;

namespace ApplicationCore.Tests.Model
{
    public class CommentTests
    {
        [Fact]
        public void AvatarIsOptional()
        {
            var avatarUri = "http://www.example.com/avatar.png";
            var commentWithAvatar = new Comment(
                "post-id", "This is a message", "My Name", avatar: avatarUri);
            var commentWithoutAvatar = new Comment(
                "post-id", "This is a message", "My Name", avatar: null!);

            Assert.Equal(avatarUri, commentWithAvatar.Avatar!.AbsoluteUri);
            Assert.Equal(null!, commentWithoutAvatar.Avatar);
        }

        [Fact]
        public void ScoreIsOptional()
        {
            var commentWithScore = new Comment(
                "post-id", "This is a message", "My Name", score: "0.75");
            var commentWithoutScore = new Comment(
                "post-id", "This is a message", "My Name", score: null!);

            Assert.Equal("0.75", commentWithScore.Score);
            Assert.Equal("Not configured", commentWithoutScore.Score);
            Assert.Equal("Not configured", commentWithoutScore.WithScore(null!).Score);
        }
    }
}
